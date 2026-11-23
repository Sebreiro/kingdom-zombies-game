'use strict';
const fs=require('node:fs'),path=require('node:path');
const {isDeepStrictEqual:equal}=require('node:util');
const {spawnSync}=require('node:child_process');
const {setTimeout:delay}=require('node:timers/promises');
const content=require('../content.cjs'),runner=require('../check.cjs');
const {ROOT,INDEX,EXPECTED,hash,need,inside,bytes,document,expected,validate,write,publish,readPublished}=content;
function snapshot(){return Object.fromEntries([INDEX,EXPECTED,'tools/phase4/content.cjs','tools/phase4/cases/provenance.cjs',
  'tools/phase4/check.cjs','src/godot-game/content/packs/tracer.json','src/godot-game/assets/common/bow.png',
  'production/phase4/tracer-source.json','src/godot-game/inspection/Inspection.tscn'].map(name=>[name,hash(bytes(ROOT,name))]));}
const LEGACY_PARTIAL={"content/pack-index.json":"ewogICJzY2hlbWFWZXJzaW9uIjogMSwKICAiaWQiOiAia2luZ2RvbS16b21iaWVzLnYwLjEiLAogICJ2ZXJzaW9uIjogMSwKICAic3RhdHVzIjogImluY29tcGxldGUiLAogICJhY2NlcHRhbmNlT3duZXIiOiAiMDQtMzIvMDQtMzMiLAogICJleHBlY3RlZE9ibGlnYXRpb25zIjogewogICAgInBhdGgiOiAic3JjL2dvZG90LWdhbWUvY29udGVudC9leHBlY3RlZC1vYmxpZ2F0aW9ucy5qc29uIiwKICAgICJieXRlcyI6IDI1MzQ1LAogICAgInNoYTI1NiI6ICJhNGQ5MjQzYmRhMWI1MTJiNTljMTg5YTNiZGM0OGE3Y2NjYzgxNzFhMjE4YmJmYzVhZTE2YjAwMjc0ZjJmZGE1IgogIH0sCiAgInNoYXJkcyI6IFsKICAgIHsKICAgICAgImlkIjogInRyYWNlciIsCiAgICAgICJvd25lclBsYW4iOiAiMDQtMDEiLAogICAgICAic3RhdHVzIjogInNlbGVjdGVkIiwKICAgICAgInZlcnNpb24iOiAxLAogICAgICAicGF0aCI6ICJzcmMvZ29kb3QtZ2FtZS9jb250ZW50L3BhY2tzL3RyYWNlci5qc29uIiwKICAgICAgImJ5dGVzIjogMjQzMCwKICAgICAgInNoYTI1NiI6ICJlMjFjOTMzYjM2YmFjZDVmODEyMDZiMzNmN2E5NjgwYmVkZDg5YmQ1ZTU4MGIxMmY1MDA0YWIyMTI4OTVkMWY0IgogICAgfSwKICAgIHsKICAgICAgImlkIjogImF1ZGlvLWFjdGlvbnMiLAogICAgICAib3duZXJQbGFuIjogIjA0LTIxIiwKICAgICAgInN0YXR1cyI6ICJwZW5kaW5nIiwKICAgICAgInBhdGgiOiAic3JjL2dvZG90LWdhbWUvY29udGVudC9wYWNrcy9hdWRpby1hY3Rpb25zLmpzb24iCiAgICB9LAogICAgewogICAgICAiaWQiOiAiYXVkaW8taW50ZXJmYWNlLWFtYmllbmNlIiwKICAgICAgIm93bmVyUGxhbiI6ICIwNC0yMSIsCiAgICAgICJzdGF0dXMiOiAicGVuZGluZyIsCiAgICAgICJwYXRoIjogInNyYy9nb2RvdC1nYW1lL2NvbnRlbnQvcGFja3MvYXVkaW8taW50ZXJmYWNlLWFtYmllbmNlLmpzb24iCiAgICB9LAogICAgewogICAgICAiaWQiOiAiYm9kaWVzLWEiLAogICAgICAib3duZXJQbGFuIjogIjA0LTA2IiwKICAgICAgInN0YXR1cyI6ICJwZW5kaW5nIiwKICAgICAgInBhdGgiOiAic3JjL2dvZG90LWdhbWUvY29udGVudC9wYWNrcy9ib2RpZXMtYS5qc29uIgogICAgfSwKICAgIHsKICAgICAgImlkIjogImJvZGllcy1iIiwKICAgICAgIm93bmVyUGxhbiI6ICIwNC0wNyIsCiAgICAgICJzdGF0dXMiOiAicGVuZGluZyIsCiAgICAgICJwYXRoIjogInNyYy9nb2RvdC1nYW1lL2NvbnRlbnQvcGFja3MvYm9kaWVzLWIuanNvbiIKICAgIH0sCiAgICB7CiAgICAgICJpZCI6ICJib2RpZXMtYyIsCiAgICAgICJvd25lclBsYW4iOiAiMDQtMDciLAogICAgICAic3RhdHVzIjogInBlbmRpbmciLAogICAgICAicGF0aCI6ICJzcmMvZ29kb3QtZ2FtZS9jb250ZW50L3BhY2tzL2JvZGllcy1jLmpzb24iCiAgICB9LAogICAgewogICAgICAiaWQiOiAiY2FtcC1hY2Nlc3MiLAogICAgICAib3duZXJQbGFuIjogIjA0LTExIiwKICAgICAgInN0YXR1cyI6ICJwZW5kaW5nIiwKICAgICAgInBhdGgiOiAic3JjL2dvZG90LWdhbWUvY29udGVudC9wYWNrcy9jYW1wLWFjY2Vzcy5qc29uIgogICAgfSwKICAgIHsKICAgICAgImlkIjogImNvbW1vbiIsCiAgICAgICJvd25lclBsYW4iOiAiMDQtMDQiLAogICAgICAic3RhdHVzIjogInBlbmRpbmciLAogICAgICAicGF0aCI6ICJzcmMvZ29kb3QtZ2FtZS9jb250ZW50L3BhY2tzL2NvbW1vbi5qc29uIgogICAgfSwKICAgIHsKICAgICAgImlkIjogImNvbXBhdGliaWxpdHkiLAogICAgICAib3duZXJQbGFuIjogIjA0LTIyIiwKICAgICAgInN0YXR1cyI6ICJwZW5kaW5nIiwKICAgICAgInBhdGgiOiAic3JjL2dvZG90LWdhbWUvY29udGVudC9wYWNrcy9jb21wYXRpYmlsaXR5Lmpzb24iCiAgICB9LAogICAgewogICAgICAiaWQiOiAiY29ycHNlcy13aWxkbGlmZSIsCiAgICAgICJvd25lclBsYW4iOiAiMDQtMTAiLAogICAgICAic3RhdHVzIjogInBlbmRpbmciLAogICAgICAicGF0aCI6ICJzcmMvZ29kb3QtZ2FtZS9jb250ZW50L3BhY2tzL2NvcnBzZXMtd2lsZGxpZmUuanNvbiIKICAgIH0sCiAgICB7CiAgICAgICJpZCI6ICJkZWZlbnNlcyIsCiAgICAgICJvd25lclBsYW4iOiAiMDQtMTIiLAogICAgICAic3RhdHVzIjogInBlbmRpbmciLAogICAgICAicGF0aCI6ICJzcmMvZ29kb3QtZ2FtZS9jb250ZW50L3BhY2tzL2RlZmVuc2VzLmpzb24iCiAgICB9LAogICAgewogICAgICAiaWQiOiAiZGVsaXZlcnkiLAogICAgICAib3duZXJQbGFuIjogIjA0LTMxIiwKICAgICAgInN0YXR1cyI6ICJwZW5kaW5nIiwKICAgICAgInBhdGgiOiAic3JjL2dvZG90LWdhbWUvY29udGVudC9wYWNrcy9kZWxpdmVyeS5qc29uIgogICAgfSwKICAgIHsKICAgICAgImlkIjogImVmZmVjdHMtcG9ydGFscyIsCiAgICAgICJvd25lclBsYW4iOiAiMDQtMTMiLAogICAgICAic3RhdHVzIjogInBlbmRpbmciLAogICAgICAicGF0aCI6ICJzcmMvZ29kb3QtZ2FtZS9jb250ZW50L3BhY2tzL2VmZmVjdHMtcG9ydGFscy5qc29uIgogICAgfSwKICAgIHsKICAgICAgImlkIjogImd1YXJkcyIsCiAgICAgICJvd25lclBsYW4iOiAiMDQtMTIiLAogICAgICAic3RhdHVzIjogInBlbmRpbmciLAogICAgICAicGF0aCI6ICJzcmMvZ29kb3QtZ2FtZS9jb250ZW50L3BhY2tzL2d1YXJkcy5qc29uIgogICAgfSwKICAgIHsKICAgICAgImlkIjogIml0ZW1zLTAxLTEwIiwKICAgICAgIm93bmVyUGxhbiI6ICIwNC0wNSIsCiAgICAgICJzdGF0dXMiOiAicGVuZGluZyIsCiAgICAgICJwYXRoIjogInNyYy9nb2RvdC1nYW1lL2NvbnRlbnQvcGFja3MvaXRlbXMtMDEtMTAuanNvbiIKICAgIH0sCiAgICB7CiAgICAgICJpZCI6ICJpdGVtcy0xMS0yNCIsCiAgICAgICJvd25lclBsYW4iOiAiMDQtMDUiLAogICAgICAic3RhdHVzIjogInBlbmRpbmciLAogICAgICAicGF0aCI6ICJzcmMvZ29kb3QtZ2FtZS9jb250ZW50L3BhY2tzL2l0ZW1zLTExLTI0Lmpzb24iCiAgICB9LAogICAgewogICAgICAiaWQiOiAibWFpbi1idWlsZGluZ3MiLAogICAgICAib3duZXJQbGFuIjogIjA0LTExIiwKICAgICAgInN0YXR1cyI6ICJwZW5kaW5nIiwKICAgICAgInBhdGgiOiAic3JjL2dvZG90LWdhbWUvY29udGVudC9wYWNrcy9tYWluLWJ1aWxkaW5ncy5qc29uIgogICAgfSwKICAgIHsKICAgICAgImlkIjogInBsYXllci1pZGVudGl0aWVzIiwKICAgICAgIm93bmVyUGxhbiI6ICIwNC0xMCIsCiAgICAgICJzdGF0dXMiOiAicGVuZGluZyIsCiAgICAgICJwYXRoIjogInNyYy9nb2RvdC1nYW1lL2NvbnRlbnQvcGFja3MvcGxheWVyLWlkZW50aXRpZXMuanNvbiIKICAgIH0sCiAgICB7CiAgICAgICJpZCI6ICJzY2FsZS1waWxvdCIsCiAgICAgICJvd25lclBsYW4iOiAiMDQtMDQiLAogICAgICAic3RhdHVzIjogInBlbmRpbmciLAogICAgICAicGF0aCI6ICJzcmMvZ29kb3QtZ2FtZS9jb250ZW50L3BhY2tzL3NjYWxlLXBpbG90Lmpzb24iCiAgICB9LAogICAgewogICAgICAiaWQiOiAidGV4dC1kYXRhIiwKICAgICAgIm93bmVyUGxhbiI6ICIwNC0yMiIsCiAgICAgICJzdGF0dXMiOiAicGVuZGluZyIsCiAgICAgICJwYXRoIjogInNyYy9nb2RvdC1nYW1lL2NvbnRlbnQvcGFja3MvdGV4dC1kYXRhLmpzb24iCiAgICB9LAogICAgewogICAgICAiaWQiOiAidWktY2FtcCIsCiAgICAgICJvd25lclBsYW4iOiAiMDQtMTYiLAogICAgICAic3RhdHVzIjogInBlbmRpbmciLAogICAgICAicGF0aCI6ICJzcmMvZ29kb3QtZ2FtZS9jb250ZW50L3BhY2tzL3VpLWNhbXAuanNvbiIKICAgIH0sCiAgICB7CiAgICAgICJpZCI6ICJ1aS1pZGVudGl0eSIsCiAgICAgICJvd25lclBsYW4iOiAiMDQtMjgiLAogICAgICAic3RhdHVzIjogInBlbmRpbmciLAogICAgICAicGF0aCI6ICJzcmMvZ29kb3QtZ2FtZS9jb250ZW50L3BhY2tzL3VpLWlkZW50aXR5Lmpzb24iCiAgICB9LAogICAgewogICAgICAiaWQiOiAidWktaW52ZW50b3J5IiwKICAgICAgIm93bmVyUGxhbiI6ICIwNC0xNSIsCiAgICAgICJzdGF0dXMiOiAicGVuZGluZyIsCiAgICAgICJwYXRoIjogInNyYy9nb2RvdC1nYW1lL2NvbnRlbnQvcGFja3MvdWktaW52ZW50b3J5Lmpzb24iCiAgICB9LAogICAgewogICAgICAiaWQiOiAidWkta2l0IiwKICAgICAgIm93bmVyUGxhbiI6ICIwNC0xNCIsCiAgICAgICJzdGF0dXMiOiAicGVuZGluZyIsCiAgICAgICJwYXRoIjogInNyYy9nb2RvdC1nYW1lL2NvbnRlbnQvcGFja3MvdWkta2l0Lmpzb24iCiAgICB9LAogICAgewogICAgICAiaWQiOiAidWktbGVnYWN5LXN0YXNoIiwKICAgICAgIm93bmVyUGxhbiI6ICIwNC0xNSIsCiAgICAgICJzdGF0dXMiOiAicGVuZGluZyIsCiAgICAgICJwYXRoIjogInNyYy9nb2RvdC1nYW1lL2NvbnRlbnQvcGFja3MvdWktbGVnYWN5LXN0YXNoLmpzb24iCiAgICB9LAogICAgewogICAgICAiaWQiOiAidWktbWFwLXdhcm5pbmdzIiwKICAgICAgIm93bmVyUGxhbiI6ICIwNC0xNiIsCiAgICAgICJzdGF0dXMiOiAicGVuZGluZyIsCiAgICAgICJwYXRoIjogInNyYy9nb2RvdC1nYW1lL2NvbnRlbnQvcGFja3MvdWktbWFwLXdhcm5pbmdzLmpzb24iCiAgICB9LAogICAgewogICAgICAiaWQiOiAidWktc2Vzc2lvbiIsCiAgICAgICJvd25lclBsYW4iOiAiMDQtMTciLAogICAgICAic3RhdHVzIjogInBlbmRpbmciLAogICAgICAicGF0aCI6ICJzcmMvZ29kb3QtZ2FtZS9jb250ZW50L3BhY2tzL3VpLXNlc3Npb24uanNvbiIKICAgIH0sCiAgICB7CiAgICAgICJpZCI6ICJ1aS1zZXR0aW5ncyIsCiAgICAgICJvd25lclBsYW4iOiAiMDQtMTciLAogICAgICAic3RhdHVzIjogInBlbmRpbmciLAogICAgICAicGF0aCI6ICJzcmMvZ29kb3QtZ2FtZS9jb250ZW50L3BhY2tzL3VpLXNldHRpbmdzLmpzb24iCiAgICB9LAogICAgewogICAgICAiaWQiOiAid29ybGQiLAogICAgICAib3duZXJQbGFuIjogIjA0LTEzIiwKICAgICAgInN0YXR1cyI6ICJwZW5kaW5nIiwKICAgICAgInBhdGgiOiAic3JjL2dvZG90LWdhbWUvY29udGVudC9wYWNrcy93b3JsZC5qc29uIgogICAgfQogIF0sCiAgIm9yaWdpbnMiOiBbCiAgICB7CiAgICAgICJvd25lciI6ICJzaGFyZWRhc3NldHMwLmFzc2V0cyIsCiAgICAgICJ0eXBlIjogIlNwcml0ZSIsCiAgICAgICJvYmplY3RJZCI6IDcwMDgsCiAgICAgICJuYW1lIjogInRvb2xzX2JvdyIsCiAgICAgICJvd25lclNoYTI1NiI6ICIzNjQ4NjU4ZDQ3NTE3MDY2MTUxN2M0OTA3ZTUwOTc3YTRhYjkzMjQ0MWI0MDRhNjgxY2QwY2U0ODU1OTJmM2ZmIiwKICAgICAgInJhd1NoYTI1NiI6ICIxMmE3MzAzZmEwODE1ZmI4Y2QwYWMxMDYzMGU5MzRkZDkxYjg3Nzc3YzA0MTgzOGFkYTdjZmE4MjU5ZmE0MDIzIiwKICAgICAgInN0cmVhbXMiOiBbCiAgICAgICAgewogICAgICAgICAgInBhdGgiOiAic2hhcmVkYXNzZXRzMC5hc3NldHMucmVzUyIsCiAgICAgICAgICAib2Zmc2V0IjogMjgwNzE4MTI4LAogICAgICAgICAgImJ5dGVzIjogNDE5NDMwNCwKICAgICAgICAgICJzaGEyNTYiOiAiMmVmY2NhZTg4MDAyYTAyY2E4NWYwNTBhNmQ2ZGMyZTVmZTg5NGMzMmRlODBlMzZmMzBjZDEyZjhjN2ZmYjVjMiIsCiAgICAgICAgICAicmFuZ2VTaGEyNTYiOiAiNjIwZDNhNmZkM2Y0NDFlYTRlMDJmMDdiZWY4NDc5ZjExOGQ0NTEwNzZlMjk5MTdkMjkyNzk3YmFiYjNiYWQ1MyIKICAgICAgICB9CiAgICAgIF0sCiAgICAgICJkZXBlbmRlbmNpZXMiOiBbCiAgICAgICAgewogICAgICAgICAgIm93bmVyIjogInNoYXJlZGFzc2V0czAuYXNzZXRzIiwKICAgICAgICAgICJ0eXBlIjogIlNwcml0ZUF0bGFzIiwKICAgICAgICAgICJvYmplY3RJZCI6IDI1Mzc2LAogICAgICAgICAgIm5hbWUiOiAib2JqZWN0cyIKICAgICAgICB9LAogICAgICAgIHsKICAgICAgICAgICJvd25lciI6ICJzaGFyZWRhc3NldHMwLmFzc2V0cyIsCiAgICAgICAgICAidHlwZSI6ICJUZXh0dXJlMkQiLAogICAgICAgICAgIm9iamVjdElkIjogMTc4LAogICAgICAgICAgIm5hbWUiOiAic2FjdHgtMC0xMDI0eDEwMjQtVW5jb21wcmVzc2VkLW9iamVjdHMtMDc2NTRjN2IiCiAgICAgICAgfQogICAgICBdCiAgICB9CiAgXSwKICAib3V0cHV0cyI6IFsKICAgIHsKICAgICAgImlkIjogImNvbW1vbi5ib3ciLAogICAgICAibmFtZSI6ICJCb3cgKHBhcnRpYWwgbWFzdGVyKSIsCiAgICAgICJ2ZXJzaW9uIjogMSwKICAgICAgInNoYXJkIjogInRyYWNlciIsCiAgICAgICJwYXRoIjogInNyYy9nb2RvdC1nYW1lL2Fzc2V0cy9jb21tb24vYm93LnBuZyIsCiAgICAgICJieXRlcyI6IDEzNSwKICAgICAgInNoYTI1NiI6ICIwOGI1MzY5OWU0NTlkNzFjZjczZjNlN2JlODRhZjE5ZDA0ODA1ZDVhN2E0ODgxMjFlMzlhNTE0YTgxNzAxOTk5IiwKICAgICAgInJvdXRlIjogInJldXNlIiwKICAgICAgIm9yaWdpbnMiOiBbCiAgICAgICAgewogICAgICAgICAgIm93bmVyIjogInNoYXJlZGFzc2V0czAuYXNzZXRzIiwKICAgICAgICAgICJ0eXBlIjogIlNwcml0ZSIsCiAgICAgICAgICAib2JqZWN0SWQiOiA3MDA4LAogICAgICAgICAgIm5hbWUiOiAidG9vbHNfYm93IiwKICAgICAgICAgICJvd25lclNoYTI1NiI6ICIzNjQ4NjU4ZDQ3NTE3MDY2MTUxN2M0OTA3ZTUwOTc3YTRhYjkzMjQ0MWI0MDRhNjgxY2QwY2U0ODU1OTJmM2ZmIiwKICAgICAgICAgICJyYXdTaGEyNTYiOiAiMTJhNzMwM2ZhMDgxNWZiOGNkMGFjMTA2MzBlOTM0ZGQ5MWI4Nzc3N2MwNDE4MzhhZGE3Y2ZhODI1OWZhNDAyMyIsCiAgICAgICAgICAic3RyZWFtcyI6IFsKICAgICAgICAgICAgewogICAgICAgICAgICAgICJwYXRoIjogInNoYXJlZGFzc2V0czAuYXNzZXRzLnJlc1MiLAogICAgICAgICAgICAgICJvZmZzZXQiOiAyODA3MTgxMjgsCiAgICAgICAgICAgICAgImJ5dGVzIjogNDE5NDMwNCwKICAgICAgICAgICAgICAic2hhMjU2IjogIjJlZmNjYWU4ODAwMmEwMmNhODVmMDUwYTZkNmRjMmU1ZmU4OTRjMzJkZTgwZTM2ZjMwY2QxMmY4YzdmZmI1YzIiLAogICAgICAgICAgICAgICJyYW5nZVNoYTI1NiI6ICI2MjBkM2E2ZmQzZjQ0MWVhNGUwMmYwN2JlZjg0NzlmMTE4ZDQ1MTA3NmUyOTkxN2QyOTI3OTdiYWJiM2JhZDUzIgogICAgICAgICAgICB9CiAgICAgICAgICBdLAogICAgICAgICAgImRlcGVuZGVuY2llcyI6IFsKICAgICAgICAgICAgewogICAgICAgICAgICAgICJvd25lciI6ICJzaGFyZWRhc3NldHMwLmFzc2V0cyIsCiAgICAgICAgICAgICAgInR5cGUiOiAiU3ByaXRlQXRsYXMiLAogICAgICAgICAgICAgICJvYmplY3RJZCI6IDI1Mzc2LAogICAgICAgICAgICAgICJuYW1lIjogIm9iamVjdHMiCiAgICAgICAgICAgIH0sCiAgICAgICAgICAgIHsKICAgICAgICAgICAgICAib3duZXIiOiAic2hhcmVkYXNzZXRzMC5hc3NldHMiLAogICAgICAgICAgICAgICJ0eXBlIjogIlRleHR1cmUyRCIsCiAgICAgICAgICAgICAgIm9iamVjdElkIjogMTc4LAogICAgICAgICAgICAgICJuYW1lIjogInNhY3R4LTAtMTAyNHgxMDI0LVVuY29tcHJlc3NlZC1vYmplY3RzLTA3NjU0YzdiIgogICAgICAgICAgICB9CiAgICAgICAgICBdCiAgICAgICAgfQogICAgICBdLAogICAgICAicHJvZHVjdGlvbiI6IHsKICAgICAgICAicGF0aCI6ICJwcm9kdWN0aW9uL3BoYXNlNC90cmFjZXItc291cmNlLmpzb24iLAogICAgICAgICJieXRlcyI6IDE0NTQxLAogICAgICAgICJzaGEyNTYiOiAiMDM2NzNlMmY4OTIzNjkyNjNhNTRmY2E2YWVlMjY2ODRiYzVmM2YwYjlhZWU1NDBmM2MwMDFkMTkwYmQ0NWZhYyIKICAgICAgfSwKICAgICAgImRlcGVuZGVuY2llcyI6IFsKICAgICAgICB7CiAgICAgICAgICAiaWQiOiAiaW5zcGVjdGlvbi5ib3ciLAogICAgICAgICAgInBhdGgiOiAic3JjL2dvZG90LWdhbWUvaW5zcGVjdGlvbi9JbnNwZWN0aW9uLnRzY24iLAogICAgICAgICAgImJ5dGVzIjogOTkzLAogICAgICAgICAgInNoYTI1NiI6ICI2NjcwMmVmYjliZjkzOTBmNjQ1NDMxNmNlMGVjNWJjNzgwOGNiNWY0MzlhZGRmMDQzMGQ0NmFiY2NiNTg5NzM5IgogICAgICAgIH0KICAgICAgXSwKICAgICAgIm1ldGFkYXRhIjogewogICAgICAgICJraW5kIjogImltYWdlIiwKICAgICAgICAid2lkdGgiOiAxNiwKICAgICAgICAiaGVpZ2h0IjogMTgsCiAgICAgICAgInJnYmFTaGEyNTYiOiAiODhiOGU3MDYyZjBlYjljZmI3ODhlZjM2NTgwMjhiYTU4YTZmNjJkNDA4ZDNlMzlhZTJlMTIyOGJmYTZjODkwNSIsCiAgICAgICAgInJlZ2lzdHJhdGlvbiI6IHsKICAgICAgICAgICJsb2dpY2FsUmVjdCI6IHsKICAgICAgICAgICAgIngiOiAwLAogICAgICAgICAgICAieSI6IDAsCiAgICAgICAgICAgICJ3aWR0aCI6IDE2LAogICAgICAgICAgICAiaGVpZ2h0IjogMTgKICAgICAgICAgIH0sCiAgICAgICAgICAicGl2b3ROb3JtYWxpemVkQm90dG9tTGVmdCI6IHsKICAgICAgICAgICAgIngiOiAwLjM3NSwKICAgICAgICAgICAgInkiOiAwLjUKICAgICAgICAgIH0sCiAgICAgICAgICAic291cmNlUGl4ZWxzUGVyVW5pdCI6IDMyLAogICAgICAgICAgInNvdXJjZU9mZnNldCI6IHsKICAgICAgICAgICAgIngiOiAtMiwKICAgICAgICAgICAgInkiOiAwCiAgICAgICAgICB9LAogICAgICAgICAgInRyaW1PZmZzZXQiOiB7CiAgICAgICAgICAgICJ4IjogMCwKICAgICAgICAgICAgInkiOiAwCiAgICAgICAgICB9LAogICAgICAgICAgImF0bGFzUmVjdE9mZnNldCI6IHsKICAgICAgICAgICAgIngiOiA1NzYsCiAgICAgICAgICAgICJ5IjogNTI2CiAgICAgICAgICB9LAogICAgICAgICAgImF0bGFzVGV4dHVyZVJlY3QiOiB7CiAgICAgICAgICAgICJ4IjogNTc4LAogICAgICAgICAgICAieSI6IDUyNiwKICAgICAgICAgICAgIndpZHRoIjogMTYsCiAgICAgICAgICAgICJoZWlnaHQiOiAxOAogICAgICAgICAgfSwKICAgICAgICAgICJhdGxhc1NldHRpbmdzUmF3IjogNjcsCiAgICAgICAgICAiYWxwaGFUZXh0dXJlSWQiOiAwLAogICAgICAgICAgImRpc3BsYXkiOiB7CiAgICAgICAgICAgICJjZW50ZXJlZCI6IGZhbHNlLAogICAgICAgICAgICAib2Zmc2V0VG9wTGVmdCI6IFsKICAgICAgICAgICAgICAtNiwKICAgICAgICAgICAgICAtOQogICAgICAgICAgICBdLAogICAgICAgICAgICAic2NhbGUiOiBbCiAgICAgICAgICAgICAgMSwKICAgICAgICAgICAgICAxCiAgICAgICAgICAgIF0sCiAgICAgICAgICAgICJmaWx0ZXIiOiAibmVhcmVzdCIKICAgICAgICAgIH0KICAgICAgICB9CiAgICAgIH0sCiAgICAgICJhcHBsaWNhYmlsaXR5IjogewogICAgICAgICJvYmxpZ2F0aW9ucyI6ICJJVC0wNyBwYXJ0aWFsIG1hc3RlciBvbmx5OyBpY29uL2RyYXduL3JlbGVhc2VkL2ZpdHMvY29ycHNlIHN0aWxsIHJlcXVpcmVkIiwKICAgICAgICAibGlzdGVuaW5nIjogIm5vdC1hcHBsaWNhYmxlIgogICAgICB9LAogICAgICAiaW5zcGVjdGlvbiI6IHsKICAgICAgICAicm91dGUiOiAicmVzOi8vaW5zcGVjdGlvbi9JbnNwZWN0aW9uLnRzY24iLAogICAgICAgICJkZXBlbmRlbmN5IjogImluc3BlY3Rpb24uYm93IgogICAgICB9LAogICAgICAiZXZpZGVuY2UiOiB7CiAgICAgICAgInNvdXJjZSI6ICJwZW5kaW5nIiwKICAgICAgICAibmF0aXZlIjogInBlbmRpbmciLAogICAgICAgICJsaXN0ZW5pbmciOiAibm90LWFwcGxpY2FibGUiLAogICAgICAgICJqdWRnbWVudCI6ICJwZW5kaW5nIiwKICAgICAgICAicmVjb3JkcyI6IFtdCiAgICAgIH0sCiAgICAgICJoaXN0b3JpY2FsSW5wdXRFdmlkZW5jZSI6IHsKICAgICAgICAibGV2ZWwiOiAiViIsCiAgICAgICAgImNyZWRpdCI6ICJObyBjdXJyZW50IHByb2R1Y3Rpb24vbmF0aXZlL3Zpc3VhbCBhY2NlcHRhbmNlIGNyZWRpdCIKICAgICAgfSwKICAgICAgIm9ibGlnYXRpb25zIjogWwogICAgICAgIHsKICAgICAgICAgICJpZCI6ICJJVC0wNyIsCiAgICAgICAgICAiY292ZXJhZ2UiOiAicGFydGlhbCIKICAgICAgICB9CiAgICAgIF0KICAgIH0KICBdCn0K","content/packs/tracer.json":"ewogICJzY2hlbWFWZXJzaW9uIjogMSwKICAiaWQiOiAidHJhY2VyIiwKICAidmVyc2lvbiI6IDEsCiAgInN0YXR1cyI6ICJpbmNvbXBsZXRlLXBoYXNlLXBhY2siLAogICJyZXNvdXJjZXMiOiBbCiAgICB7CiAgICAgICJpZCI6ICJjb21tb24uYm93IiwKICAgICAgIm5hbWUiOiAiQm93IiwKICAgICAgInZlcnNpb24iOiAxLAogICAgICAicGF0aCI6ICJhc3NldHMvY29tbW9uL2Jvdy5wbmciLAogICAgICAic2NlbmUiOiAiaW5zcGVjdGlvbi9JbnNwZWN0aW9uLnRzY24iLAogICAgICAic2hhMjU2IjogIjA4YjUzNjk5ZTQ1OWQ3MWNmNzNmM2U3YmU4NGFmMTlkMDQ4MDVkNWE3YTQ4ODEyMWUzOWE1MTRhODE3MDE5OTkiLAogICAgICAic291cmNlIjogewogICAgICAgICJvd25lciI6ICJzaGFyZWRhc3NldHMwLmFzc2V0cyIsCiAgICAgICAgInR5cGUiOiAiU3ByaXRlIiwKICAgICAgICAib2JqZWN0SWQiOiA3MDA4LAogICAgICAgICJuYW1lIjogInRvb2xzX2JvdyIsCiAgICAgICAgImF0bGFzSWQiOiAyNTM3NiwKICAgICAgICAiYXRsYXNOYW1lIjogIm9iamVjdHMiLAogICAgICAgICJ0ZXh0dXJlSWQiOiAxNzgsCiAgICAgICAgInRleHR1cmVOYW1lIjogInNhY3R4LTAtMTAyNHgxMDI0LVVuY29tcHJlc3NlZC1vYmplY3RzLTA3NjU0YzdiIiwKICAgICAgICAic3RyZWFtIjogewogICAgICAgICAgIm9mZnNldCI6IDI4MDcxODEyOCwKICAgICAgICAgICJzaXplIjogNDE5NDMwNCwKICAgICAgICAgICJwYXRoIjogInNoYXJlZGFzc2V0czAuYXNzZXRzLnJlc1MiCiAgICAgICAgfQogICAgICB9LAogICAgICAicmVnaXN0cmF0aW9uIjogewogICAgICAgICJsb2dpY2FsUmVjdCI6IHsKICAgICAgICAgICJ4IjogMCwKICAgICAgICAgICJ5IjogMCwKICAgICAgICAgICJ3aWR0aCI6IDE2LAogICAgICAgICAgImhlaWdodCI6IDE4CiAgICAgICAgfSwKICAgICAgICAicGl2b3ROb3JtYWxpemVkQm90dG9tTGVmdCI6IHsKICAgICAgICAgICJ4IjogMC4zNzUsCiAgICAgICAgICAieSI6IDAuNQogICAgICAgIH0sCiAgICAgICAgInNvdXJjZVBpeGVsc1BlclVuaXQiOiAzMiwKICAgICAgICAic291cmNlT2Zmc2V0IjogewogICAgICAgICAgIngiOiAtMiwKICAgICAgICAgICJ5IjogMAogICAgICAgIH0sCiAgICAgICAgInRyaW1PZmZzZXQiOiB7CiAgICAgICAgICAieCI6IDAsCiAgICAgICAgICAieSI6IDAKICAgICAgICB9LAogICAgICAgICJhdGxhc1JlY3RPZmZzZXQiOiB7CiAgICAgICAgICAieCI6IDU3NiwKICAgICAgICAgICJ5IjogNTI2CiAgICAgICAgfSwKICAgICAgICAiYXRsYXNUZXh0dXJlUmVjdCI6IHsKICAgICAgICAgICJ4IjogNTc4LAogICAgICAgICAgInkiOiA1MjYsCiAgICAgICAgICAid2lkdGgiOiAxNiwKICAgICAgICAgICJoZWlnaHQiOiAxOAogICAgICAgIH0sCiAgICAgICAgImF0bGFzU2V0dGluZ3NSYXciOiA2NywKICAgICAgICAiYWxwaGFUZXh0dXJlSWQiOiAwLAogICAgICAgICJkaXNwbGF5IjogewogICAgICAgICAgImNlbnRlcmVkIjogZmFsc2UsCiAgICAgICAgICAib2Zmc2V0VG9wTGVmdCI6IFsKICAgICAgICAgICAgLTYsCiAgICAgICAgICAgIC05CiAgICAgICAgICBdLAogICAgICAgICAgInNjYWxlIjogWwogICAgICAgICAgICAxLAogICAgICAgICAgICAxCiAgICAgICAgICBdLAogICAgICAgICAgImZpbHRlciI6ICJuZWFyZXN0IgogICAgICAgIH0KICAgICAgfSwKICAgICAgImltYWdlIjogewogICAgICAgICJ3aWR0aCI6IDE2LAogICAgICAgICJoZWlnaHQiOiAxOCwKICAgICAgICAibW9kZSI6ICJSR0JBIiwKICAgICAgICAiYWxwaGFCQm94IjogWwogICAgICAgICAgNiwKICAgICAgICAgIDEsCiAgICAgICAgICAxMCwKICAgICAgICAgIDE3CiAgICAgICAgXSwKICAgICAgICAidmlzaWJsZVBpeGVscyI6IDQyLAogICAgICAgICJyZ2JhU2hhMjU2IjogIjg4YjhlNzA2MmYwZWI5Y2ZiNzg4ZWYzNjU4MDI4YmE1OGE2ZjYyZDQwOGQzZTM5YWUyZTEyMjhiZmE2Yzg5MDUiLAogICAgICAgICJwbmdTaGEyNTYiOiAiMDhiNTM2OTllNDU5ZDcxY2Y3M2YzZTdiZTg0YWYxOWQwNDgwNWQ1YTdhNDg4MTIxZTM5YTUxNGE4MTcwMTk5OSIsCiAgICAgICAgInBuZ0J5dGVzIjogMTM1CiAgICAgIH0sCiAgICAgICJyb3V0ZSI6ICJyZXVzZWQtbmF0aXZlLXBpeGVscyIsCiAgICAgICJ2YWxpZGF0aW9uIjogewogICAgICAgICJzb3VyY2UiOiAidmVyaWZpZWQtZXhwb3J0IiwKICAgICAgICAiaGVhZGxlc3MiOiAicnVuLXNvdXJjZS50cmFjZXItZm9yLWN1cnJlbnQtZXZpZGVuY2UiLAogICAgICAgICJ2aXN1YWwiOiAicGVuZGluZy0wNC0zMiIKICAgICAgfSwKICAgICAgIm9ibGlnYXRpb25zIjogWwogICAgICAgICJJVC0wNyIKICAgICAgXSwKICAgICAgIm9ibGlnYXRpb25TdGF0dXMiOiAicGFydGlhbC1tYXN0ZXItb25seSIKICAgIH0KICBdCn0K","inspection/Inspection.tscn":"W2dkX3NjZW5lIGxvYWRfc3RlcHM9MiBmb3JtYXQ9M10KCltleHRfcmVzb3VyY2UgdHlwZT0iVGV4dHVyZTJEIiBwYXRoPSJyZXM6Ly9hc3NldHMvY29tbW9uL2Jvdy5wbmciIGlkPSIxX2JvdyJdCgpbbm9kZSBuYW1lPSJJbnNwZWN0aW9uIiB0eXBlPSJOb2RlMkQiXQoKW25vZGUgbmFtZT0iQm93IiB0eXBlPSJTcHJpdGUyRCIgcGFyZW50PSIuIl0KdGV4dHVyZV9maWx0ZXIgPSAxCnBvc2l0aW9uID0gVmVjdG9yMig2NDAsIDM2MCkKdGV4dHVyZSA9IEV4dFJlc291cmNlKCIxX2JvdyIpCmNlbnRlcmVkID0gZmFsc2UKb2Zmc2V0ID0gVmVjdG9yMigtNiwgLTkpCgpbbm9kZSBuYW1lPSJTdW1tYXJ5IiB0eXBlPSJWQm94Q29udGFpbmVyIiBwYXJlbnQ9Ii4iXQpvZmZzZXRfbGVmdCA9IDI0LjAKb2Zmc2V0X3RvcCA9IDI0LjAKb2Zmc2V0X3JpZ2h0ID0gNzIwLjAKb2Zmc2V0X2JvdHRvbSA9IDE4NC4wCgpbbm9kZSBuYW1lPSJUaXRsZSIgdHlwZT0iTGFiZWwiIHBhcmVudD0iU3VtbWFyeSJdCmxheW91dF9tb2RlID0gMgp0ZXh0ID0gIktpbmdkb21ab21iaWVzIOKAlCBSZXNvdXJjZSBpbnNwZWN0aW9uIgoKW25vZGUgbmFtZT0iUmVzb3VyY2VJZCIgdHlwZT0iTGFiZWwiIHBhcmVudD0iU3VtbWFyeSJdCmxheW91dF9tb2RlID0gMgp0ZXh0ID0gImNvbW1vbi5ib3ciCgpbbm9kZSBuYW1lPSJSZXNvdXJjZU5hbWUiIHR5cGU9IkxhYmVsIiBwYXJlbnQ9IlN1bW1hcnkiXQpsYXlvdXRfbW9kZSA9IDIKdGV4dCA9ICJCb3ciCgpbbm9kZSBuYW1lPSJTdGF0dXMiIHR5cGU9IkxhYmVsIiBwYXJlbnQ9IlN1bW1hcnkiXQpsYXlvdXRfbW9kZSA9IDIKdGV4dCA9ICJTb3VyY2UvaGVhZGxlc3MgdmVyaWZpZWQ7IHZpc3VhbCByZXZpZXcgcGVuZGluZyIKCltub2RlIG5hbWU9IlBhY2tTdGF0dXMiIHR5cGU9IkxhYmVsIiBwYXJlbnQ9IlN1bW1hcnkiXQpsYXlvdXRfbW9kZSA9IDIKdGV4dCA9ICJQYWNrIGluY29tcGxldGUg4oCUIHRyYWNlciBtYXN0ZXIgb25seSIK"};
function legacyFixture(){return {index:JSON.parse(Buffer.from(LEGACY_PARTIAL['content/pack-index.json'],'base64')),files:new Map(Object.entries(LEGACY_PARTIAL).map(([name,value])=>['src/godot-game/'+name,Buffer.from(value,'base64')]))};}
function subject(context,name){
  const root=inside(context.directory,name);fs.mkdirSync(root);const fixture=legacyFixture(),index=fixture.index;
  const references=[index.expectedObligations,...index.shards.filter(x=>x.status==='selected'),...index.outputs.flatMap(x=>[x,x.production,...x.dependencies])];
  for(const ref of references){if(!fixture.files.has(ref.path))fixture.files.set(ref.path,bytes(ROOT,ref.path));}
  for(const [file,value]of fixture.files)write(root,file,value);
  const proof=validate(root,structuredClone(index));need(proof.valid.length===1&&!proof.diagnostics.some(x=>x.code==='OUTPUT_INVALID'),'LEGACY_PARTIAL_FIXTURE_INVALID');
  write(context.directory,name+'-fixture.json',{scope:'Independent exact historical partial one-bow fixture; no actual current-fullpack credit',files:[...fixture.files].map(([path,value])=>({path,bytes:value.length,sha256:hash(value)}))});return {root,index};
}
function failure(context,name,code,action){
  let caught;try{action();}catch(error){caught=error;}
  write(context.directory,name+'.json',{caseId:context.caseId,nonce:context.nonce,subjectStatus:caught?'fail':'pass',
    expectedCode:code,diagnostic:caught?.message??null,expectedRejectionObserved:!!caught?.message.includes(code)});
  need(caught?.message.includes(code),'CONTROL_WRONG_RESULT',name+' '+caught?.message);return {name,code,subjectStatus:'fail'};
}
function invalid(context,name,code,edit){
  const copy=subject(context,name),candidate=structuredClone(copy.index);edit(candidate,copy.root);
  const result=validate(copy.root,candidate);const failed=result.diagnostics.find(x=>x.code==='OUTPUT_INVALID'&&x.message.includes(code));
  write(context.directory,name+'-outcome.json',{nonce:context.nonce,subjectStatus:'fail',expectedCode:code,result:{...result,files:undefined}});
  need(failed,'CONTROL_WRONG_RESULT',name+' '+JSON.stringify(result.diagnostics));return {name,code,subjectStatus:'fail'};
}
function reference(root,name){const value=bytes(root,name);return {path:name,bytes:value.length,sha256:hash(value)};}
function sharedSubject(context){
  const copy=subject(context,'shared'),other=structuredClone(copy.index.outputs[0]);
  other.id='heldout.second-bow';other.name='Independent shared obligation';other.sharedFileId='shared.bow-pixels';
  other.obligations=[{id:'FX-01',coverage:'partial'}];copy.index.outputs[0].sharedFileId=other.sharedFileId;
  const production=document(copy.root,other.production.path);production.id=other.id;
  write(copy.root,'production/phase4/heldout-second.json',production);other.production=reference(copy.root,'production/phase4/heldout-second.json');
  copy.index.outputs.push(other);const shard=copy.index.shards[0],body=document(copy.root,shard.path),binding=structuredClone(body.resources[0]);
  binding.id=other.id;body.resources.push(binding);fs.writeFileSync(inside(copy.root,shard.path),JSON.stringify(body));
  Object.assign(shard,reference(copy.root,shard.path));return copy;
}
async function provenance(context){
  const before=snapshot(),register=expected(),legacy=subject(context,'legacy-baseline'),index=legacy.index,sourceRoot=legacy.root,production=validate(sourceRoot,structuredClone(index));runner.validate(ROOT);
  const retained=await content.archive(ROOT,{path:'production/phase4/common-selected.zip',bytes:12308215,sha256:'2f66618305221d32d503bba04be755ae38e606408a7c5d9d8f65a1db19d9e6d9'});
  write(context.directory,'current-common-archive.json',retained);
  need(register.obligations.length===180&&register.obligations.some(x=>x.id==='TXT-01')&&register.obligations.some(x=>x.id==='TXT-02'),'CENSUS_REQUIRED');
  need(production.valid.length===1&&production.covered.length===0&&!production.complete&&production.diagnostics.filter(x=>x.code==='OBLIGATION_INCOMPLETE').length===180,'PARTIAL_PACK_REQUIRED');
  write(context.directory,'inputs.json',before);write(context.directory,'pack-diagnostics.json',{...production,files:undefined});const controls=[];
  const donors=[
    {owner:'heldout-a.assets',type:'Sprite',objectId:48,name:'same_name',ownerSha256:'a'.repeat(64),rawSha256:'1'.repeat(64)},
    {owner:'heldout-b.assets',type:'Sprite',objectId:48,name:'same_name',ownerSha256:'b'.repeat(64),rawSha256:'2'.repeat(64)},
    {owner:'heldout-a.assets',type:'Texture2D',objectId:49,name:'same_name',ownerSha256:'a'.repeat(64),rawSha256:'3'.repeat(64)}];
  for(const selected of donors)need(equal(content.resolveOrigin([...donors].reverse(),selected),selected),'H01_ORIGIN_RESOLUTION');
  controls.push(failure(context,'h01-wrong-owner','ORIGIN_NOT_EXACT',()=>content.resolveOrigin(donors,{...donors[0],owner:'heldout-b.assets'})));
  const shared=sharedSubject(context),sharedProof=validate(shared.root,structuredClone(shared.index));
  need(sharedProof.valid.length===2&&sharedProof.valid[0].id!==sharedProof.valid[1].id&&sharedProof.valid[1].obligations[0].id==='FX-01','H01_SHARED_IDENTITY');
  const notShared=structuredClone(shared.index);delete notShared.outputs[1].sharedFileId;
  need(validate(shared.root,notShared).diagnostics.some(x=>x.message?.includes('SHARED_OUTPUT_NOT_EXPLICIT')),'H01_IMPLICIT_SHARING');
  for(const [name,code,edit]of [
    ['missing-origin','ORIGIN_REQUIRED',x=>{x.outputs[0].origins=[];}],
    ['missing-version','VERSION_REQUIRED',x=>{delete x.outputs[0].version;}],
    ['missing-recipe','FILE_REFERENCE_REQUIRED',x=>{delete x.outputs[0].production;}],
    ['missing-hash','HASH_REQUIRED',x=>{delete x.outputs[0].sha256;}],
    ['missing-dependencies','DEPENDENCIES_REQUIRED',x=>{delete x.outputs[0].dependencies;}],
    ['missing-source-dependencies','SOURCE_DEPENDENCY_MISSING',x=>{x.origins[0].dependencies=[];x.outputs[0].origins[0].dependencies=[];}],
    ['missing-source-stream','SOURCE_STREAM_MISSING',x=>{x.origins[0].streams=[];x.outputs[0].origins[0].streams=[];}],
    ['missing-evidence','EVIDENCE_REQUIRED',x=>{delete x.outputs[0].evidence;}],
    ['false-source-evidence','EVIDENCE_MISSING',x=>{x.outputs[0].evidence.source='verified';}],
    ['borrowed-source-evidence','EVIDENCE_BINDING',(x,root)=>{
      write(root,'stale.json',{status:'pass',nonce:'old-run',caseId:'source.provenance',outputs:[{id:'common.bow',version:1,sha256:'0'.repeat(64)}]});
      x.outputs[0].evidence.source='verified';x.outputs[0].evidence.records=[{kind:'source',caseId:'source.provenance',invocation:'current-run',...reference(root,'stale.json')}];}],
    ['missing-file','ENOENT',(_,root)=>fs.unlinkSync(inside(root,'src/godot-game/assets/common/bow.png'))],
    ['hash-drift','FILE_HASH_MISMATCH',(_,root)=>fs.writeFileSync(inside(root,'src/godot-game/assets/common/bow.png'),Buffer.from('changed'))],
    ['path-escape','PATH_INVALID',x=>{x.outputs[0].path='../bow.png';}],
    ['path-case','PATH_CASE',x=>{x.outputs[0].path='src/godot-game/assets/common/Bow.png';}],
    ['id-case','OUTPUT_ID_DUPLICATE_OR_INVALID',x=>{x.outputs[0].id='Common.bow';}],
    ['id-nfd','IDENTITY_INVALID',x=>{x.outputs[0].id='bow.e\u0301';}]
  ])controls.push(invalid(context,'h02-h03-'+name,code,edit));
  controls.push(failure(context,'h02-false-pack-completion','FALSE_COMPLETION',()=>validate(sourceRoot,{...structuredClone(index),status:'complete'})));
  controls.push(failure(context,'h03-invalid-utf8','encoded data',()=>content.parse(Buffer.from([0xff]),'bad-utf8')));
  controls.push(failure(context,'h03-duplicate-json-key','JSON_DUPLICATE_KEY',()=>content.parse(Buffer.from('{"id":1,"id":2}'),'duplicate')));
  controls.push(failure(context,'h03-case-alias-source','ORIGIN_NOT_EXACT',()=>content.resolveOrigin(donors,{...donors[0],owner:'Heldout-a.assets'})));
  const shuffled=structuredClone(shared.index);shuffled.outputs.reverse();shuffled.shards.reverse();shuffled.origins.reverse();
  need(equal(validate(shared.root,shuffled).valid.sort((a,b)=>a.id<b.id?-1:1),sharedProof.valid.sort((a,b)=>a.id<b.id?-1:1)),'H04_ENUMERATION');
  const publicationRoot=path.join(context.directory,'publication'),first=publish({sourceRoot,index,publicationRoot,invocation:context.nonce+'-first'});
  const pointer=hash(bytes(publicationRoot,'current.json'));
  controls.push(failure(context,'h05-interrupted-before-validation','HELDOUT_INTERRUPT',()=>publish({sourceRoot,index,publicationRoot,invocation:context.nonce+'-interrupt',observe:step=>{if(step==='staged')throw new Error('HELDOUT_INTERRUPT');}})));
  need(hash(bytes(publicationRoot,'current.json'))===pointer,'H05_POINTER_CHANGED');
  let competing=false;publish({sourceRoot,index,publicationRoot,invocation:context.nonce+'-lock',observe:step=>{if(step==='locked'){
    controls.push(failure(context,'h05-competing-lock','PUBLICATION_LOCKED',()=>publish({sourceRoot,index,publicationRoot,invocation:context.nonce+'-competitor'})));competing=true;}}});
  need(competing,'H05_COMPETITOR_NOT_RUN');const replay=publish({sourceRoot,index,publicationRoot,invocation:context.nonce+'-replay'});
  need(equal(first.outputs,replay.outputs)&&readPublished(publicationRoot).index.outputs.length===1,'H33_REPLAY_IDENTITIES');
  const merge1=content.merge(index,{outputs:index.outputs,origins:index.origins,shards:index.shards});
  need(merge1.outputs.length===1&&equal(content.merge(merge1,index),merge1),'H33_MERGE_IDEMPOTENCY');
  need(equal(snapshot(),before),'PRODUCTION_CHANGED');write(context.directory,'inputs-after.json',snapshot());
  return {expected:180,validOutputs:production.valid.length,completedObligations:0,status:'incomplete',controls,
    currentRetainedSource:{path:retained.path,sha256:retained.sha256,members:retained.members.length,expandedBytes:retained.expandedBytes,scope:'Authenticated streamed retained archive only; current fullpack adoption and acceptance remain unperformed'},
    heldout:{H01:'pass-source',H02:'pass-source',H03:'pass-source',H04:'pass-source',H05:'pass-source-basic',H33:'pass-source-basic'},
    finalNativeListeningJudgment:'pending-04-32/04-33',originals:'No authoring donor access; tracked originals/source bundle unchanged'};
}
const definition={group:'source',title:'Exact independent census, provenance and staged publication',importance:'Critical',
  rationale:'False identity/coverage or publication can corrupt production provenance',needsGameWindow:false,runGroup:'G0',order:3,
  requirements:['ASSET-01','ASSET-02','ASSET-05'],acceptance:['AC-01','AC-02','AC-03','AC-16','AC-19','AC-22'],
  setup:'Tracked partial bow shard/source and independent 180-ID register; fresh owned evidence directory; no donor root',
  actions:['Validate actual selected bytes, dependencies and exact origins','Run independently selected duplicate-owner/type and shared-output controls',
    'Reject missing provenance/evidence and ordinal/UTF-8/path errors','Shuffle enumeration','Interrupt and compete before commit','Repeat publication without duplicate stable identities'],
  expected:'180 expected, zero completed obligations, one usable partial output; every deliberate bad subject fails; unaffected output remains inspectable',
  reset:'Run-owned copies and generations only; no game/audio; preserve failure evidence and originals',
  evidence:['inputs.json','pack-diagnostics.json','h01-wrong-owner.json','h05-interrupted-before-validation.json','h05-competing-lock.json','inputs-after.json','result.json']};
runner.registerCase('source.provenance',definition,provenance);
async function waitForPublicationMarker(directory,name){
  const deadline=Date.now()+5000;
  while(!fs.existsSync(inside(directory,name))){
    need(Date.now()<deadline,'PUBLICATION_RACE_MARKER_TIMEOUT',name);
    await delay(25);
  }
  return document(directory,name);
}
async function publicationRecoveryRace(context,{sourceRoot,publicationRoot,script}){
  const dead=context.nonce+'-race-dead',fresh=context.nonce+'-race-new';
  const pointer=hash(bytes(publicationRoot,'current.json'));
  const interrupted=await runner.command(context,'race-dead-owner',process.execPath,[script,'--publish-worker',
    sourceRoot,publicationRoot,dead,'before-commit'],{interruptMarker:'PUBLICATION_PAUSED '+dead+' before-commit'}).catch(error=>error);
  need(interrupted.message?.includes('LOADER_INTERRUPTED'),'PUBLICATION_RACE_DEAD_OWNER');
  const harness=`
    const [script,root,publication,invocation,mode]=process.argv.slice(1);
    const fs=require('node:fs'),content=require(script),directory=process.cwd();
    const wait=(name)=>{const limit=Date.now()+15000;
      while(!fs.existsSync(content.inside(directory,name))){
        if(Date.now()>=limit)throw new Error('PUBLICATION_RACE_RELEASE_TIMEOUT '+name);
        Atomics.wait(new Int32Array(new SharedArrayBuffer(4)),0,0,20);
      }
    };
    const observe=(step,key)=>{
      content.write(directory,key+'-ready.json',{pid:process.pid,invocation,step});
      wait(key+'-go.json');
    };
    async function run(){
      if(mode==='new-writer'){
        const result=await content.publish({sourceRoot:root,index:content.document(root,content.INDEX),
          publicationRoot:publication,invocation,observe:step=>{if(step==='locked')observe(step,'writer');}});
        console.log(JSON.stringify(result));
      }else{
        const result=content.recoverPublication({publicationRoot:publication,invocation,
          observe:step=>{if(step==='owner-read'&&mode==='stale-recoverer')observe(step,'recoverer');}});
        console.log(JSON.stringify(result));
      }
    }
    run().catch(error=>{console.error(error.message);process.exitCode=1;});
  `;
  const args=(invocation,mode)=>['-e',harness,script,sourceRoot,publicationRoot,invocation,mode];
  const launched=[];
  function launch(name,invocation,mode){
    const child=runner.command(context,name,process.execPath,args(invocation,mode),{timeout:22000}).catch(error=>error);
    launched.push(child);return child;
  }
  function permit(name){write(context.directory,name+'-go.json',{release:true});}
  function blocked(args,code){
    const result=spawnSync(process.execPath,args,{cwd:context.directory,windowsHide:true,encoding:'utf8',timeout:5000});
    need(!result.error&&result.status===1&&result.stderr.startsWith(code+' '),'PUBLICATION_RACE_CONTEST',result.stderr);
    return {exitCode:result.status,stderr:result.stderr.trim()};
  }
  const stale=launch('race-stale-recoverer',dead,'stale-recoverer');
  try{
    const staleOwner=await waitForPublicationMarker(context.directory,'recoverer-ready.json');
    need(staleOwner.pid!==process.pid&&staleOwner.invocation===dead&&staleOwner.step==='owner-read','PUBLICATION_RACE_STALE_READ');
    let refused;
    const recovered=content.recoverPublication({publicationRoot,invocation:dead,observe:step=>{
      if(step!=='before-unlock')return;
      refused={
        recoverer:blocked(args(dead,'competing-recoverer'),'RECOVERY_LOCKED'),
        writer:blocked([script,'--publish-worker',sourceRoot,publicationRoot,fresh,'none'],'Error: PUBLICATION_LOCKED')
      };
      need(document(publicationRoot,'publication.lock').invocation===dead&&
        hash(bytes(publicationRoot,'current.json'))===pointer,'PUBLICATION_RACE_BEFORE_UNLOCK');
    }});
    need(refused&&!recovered.published,'PUBLICATION_RACE_RECOVERY');
    const writer=launch('race-new-writer',fresh,'new-writer');
    const writerOwner=await waitForPublicationMarker(context.directory,'writer-ready.json');
    const liveLock=document(publicationRoot,'publication.lock');
    need(writerOwner.pid===liveLock.pid&&writerOwner.invocation===fresh&&liveLock.invocation===fresh&&
      writerOwner.step==='locked','PUBLICATION_RACE_NEW_OWNER');
    permit('recoverer');
    const rejected=await stale,record=document(context.directory,'race-stale-recoverer.command.json');
    need(rejected instanceof Error&&record.pid===staleOwner.pid&&record.code===1&&record.streamsClosed&&
      !record.cleanupError&&record.stderr.startsWith('RECOVERY_LOCK_CHANGED'),'PUBLICATION_RACE_STALE_REJECTION',record.stderr);
    need(equal(document(publicationRoot,'publication.lock'),liveLock)&&
      hash(bytes(publicationRoot,'current.json'))===pointer,'PUBLICATION_RACE_WINNER_UNCHANGED');
    permit('writer');
    const completed=await writer;
    need(!(completed instanceof Error)&&completed.pid===writerOwner.pid&&
      readPublished(publicationRoot).pointer.invocation===fresh&&
      !fs.existsSync(inside(publicationRoot,'publication.lock')),'PUBLICATION_RACE_NEW_PUBLICATION');
    const evidence={dead,recovered,refused,staleRecovererPid:staleOwner.pid,winningWriterPid:writerOwner.pid,
      currentUnchangedDuringRecovery:true,winningLockUnchanged:true,published:fresh};
    write(context.directory,'publication-recovery-race.json',evidence);
    return evidence;
  }finally{await Promise.all(launched);}
}
async function publication(context){
  const before=snapshot(),legacy=subject(context,'legacy-baseline'),index=legacy.index,sourceRoot=legacy.root,publicationRoot=path.join(context.directory,'publication'),script=path.join(ROOT,'src/godot-game/tools/asset-pipeline/content.cjs');
  write(context.directory,'inputs.json',before);const baseline=publish({sourceRoot,index,publicationRoot,invocation:context.nonce+'-baseline'});
  const observations=[];let observedCompetitor=false;
  for(const step of ['locked','staged','validated','before-commit']){
    const invocation=context.nonce+'-'+step,name='interrupted-'+step,pointer=hash(bytes(publicationRoot,'current.json'));let caught;
    try{await runner.command(context,name,process.execPath,[script,'--publish-worker',sourceRoot,publicationRoot,invocation,step],
      {timeout:10000,interruptMarker:'PUBLICATION_PAUSED '+invocation+' '+step});}catch(error){caught=error;}
    const command=document(context.directory,name+'.command.json');
    need(caught?.message.includes('LOADER_INTERRUPTED')&&command.reason==='LOADER_INTERRUPTED'&&command.streamsClosed&&
      !command.cleanupError&&command.stdout.includes('PUBLICATION_PAUSED '+invocation+' '+step),'H05_ACTUAL_INTERRUPTION',step);
    need(hash(bytes(publicationRoot,'current.json'))===pointer&&readPublished(publicationRoot).pointer.invocation===baseline.invocation,'H05_PARTIAL_PUBLICATION',step);
    failure(context,name+'-retry','PUBLICATION_LOCKED',()=>publish({sourceRoot,index,publicationRoot,invocation:context.nonce+'-retry-'+step}));
    if(step==='locked'){
      let competitor;try{await runner.command(context,'competitor',process.execPath,[script,'--publish-worker',sourceRoot,publicationRoot,context.nonce+'-competitor','none'],{timeout:10000});}catch(error){competitor=error;}
      const record=document(context.directory,'competitor.command.json');
      need(competitor&&record.code===1&&record.stderr.includes('PUBLICATION_LOCKED')&&record.streamsClosed&&!record.cleanupError,'H05_PROCESS_COMPETITION');
      write(context.directory,'competing-subject.json',{nonce:context.nonce,subjectStatus:'fail',diagnostic:'PUBLICATION_LOCKED',pid:record.pid});observedCompetitor=true;
    }
    failure(context,name+'-wrong-recovery','RECOVERY_OWNER',()=>content.recoverPublication({publicationRoot,invocation:'wrong-owner'}));
    const recovered=content.recoverPublication({publicationRoot,invocation});
    need(!recovered.published&&!fs.existsSync(inside(publicationRoot,'publication.lock'))&&hash(bytes(publicationRoot,'current.json'))===pointer,'H05_RECOVERY');
    need(!fs.existsSync(inside(publicationRoot,'stage-'+invocation))&&!fs.existsSync(inside(publicationRoot,'generations/'+invocation)),'H05_ORPHAN_CLEANUP');
    write(context.directory,name+'-result.json',{nonce:invocation,caseId:'heldout.H05',status:'interrupted',verified:false,command:name+'.command.json',recovered});
    observations.push({step,invocation,status:'interrupted',verified:false,pointerUnchanged:true,recovered});
  }
  need(observedCompetitor,'H05_COMPETITOR_NOT_RUN');
  await runner.command(context,'live-competition',process.execPath,[script,'--publish-worker',sourceRoot,publicationRoot,context.nonce+'-alive','compete'],{timeout:10000});
  const liveCompetition=document(publicationRoot,'live-competition.json');
  need(liveCompetition.subjectStatus==='fail'&&liveCompetition.observed==='PUBLICATION_LOCKED while owner is alive','H05_LIVE_COMPETITION');
  let liveRecovery=false;publish({sourceRoot,index,publicationRoot,invocation:context.nonce+'-live',observe:step=>{
    if(step==='locked'){failure(context,'live-owner-recovery','RECOVERY_OWNER_ALIVE',()=>content.recoverPublication({publicationRoot,invocation:context.nonce+'-live'}));liveRecovery=true;}}});
  need(liveRecovery,'RECOVERY_LIVE_NOT_RUN');
  const pointer=hash(bytes(publicationRoot,'current.json'));
  const empty={...structuredClone(index),outputs:[]};
  failure(context,'no-regression-to-empty','PUBLICATION_REGRESSION',()=>publish({sourceRoot,index:empty,publicationRoot,invocation:context.nonce+'-empty'}));
  const altered=structuredClone(index);altered.outputs[0].metadata.note='changed stable version';
  failure(context,'no-overwrite-stable-identity','IDENTITY_IMMUTABLE',()=>publish({sourceRoot,index:altered,publicationRoot,invocation:context.nonce+'-changed'}));
  need(hash(bytes(publicationRoot,'current.json'))===pointer,'FAILED_RUN_INHERITED_PUBLICATION');
  const broken=subject(context,'broken');fs.unlinkSync(inside(broken.root,'src/godot-game/assets/common/bow.png'));
  failure(context,'metadata-only-cannot-publish','PUBLICATION_INVALID_OUTPUT',()=>publish({sourceRoot:broken.root,index:broken.index,publicationRoot,invocation:context.nonce+'-missing'}));
  need(hash(bytes(publicationRoot,'current.json'))===pointer,'MISSING_BYTES_PUBLISHED');
  failure(context,'original-root-denied','PUBLICATION_ROOT_DENIED',()=>content.destination('D:/Work/Games/KindomCrown/recovered'));
  failure(context,'root-escape-denied','PUBLICATION_ROOT_DENIED',()=>content.destination(path.join(ROOT,'../outside')));
  failure(context,'encoding-escape-denied','PATH_INVALID',()=>content.inside(ROOT,content.runsPath()+'/%2e%2e/outside'));
  failure(context,'windows-device-denied','PATH_INVALID',()=>content.inside(ROOT,content.runsPath()+'/con.txt'));
  const junction=inside(context.directory,'reparse');fs.symlinkSync(publicationRoot,junction,process.platform==='win32'?'junction':'dir');
  try{failure(context,'reparse-denied','PATH_REPARSE',()=>content.destination(junction));}finally{fs.unlinkSync(junction);}
  const runs=[];
  for(let i=0;i<3;i++){
    const shuffled=structuredClone(index);if(i%2)shuffled.shards.reverse();
    const fresh=publish({sourceRoot,index:shuffled,publicationRoot,invocation:context.nonce+'-repeat-'+i});
    need(equal(fresh.outputs,baseline.outputs),'H33_EXPORT_IDENTITIES');const imported=readPublished(publicationRoot);
    need(imported.index.outputs.length===1&&imported.result.valid.length===1&&!imported.result.complete,'H33_IMPORT_IDENTITIES');runs.push(fresh);
  }
  const combined=sharedSubject(context);combined.index.shards.reverse();combined.index.outputs.reverse();
  const compositionRoot=path.join(context.directory,'composition');
  publish({sourceRoot:combined.root,index:combined.index,publicationRoot:compositionRoot,invocation:context.nonce+'-shared'});
  const imported=readPublished(compositionRoot);need(imported.result.valid.length===2&&imported.result.covered.length===0,'H33_SHARED_COMPOSITION');
  const forward=content.merge(index,{outputs:[],origins:[],shards:[{...index.shards.find(x=>x.status==='pending'),status:'selected',version:1,bytes:15,sha256:'a'.repeat(64)}]});
  need(forward.shards.filter(x=>x.status==='selected').length===2,'PENDING_SHARD_TRANSITION');
  need(equal(snapshot(),before),'PRODUCTION_CHANGED');write(context.directory,'inputs-after.json',snapshot());
  const recoveryRace=await publicationRecoveryRace(context,{sourceRoot,publicationRoot,script});
  write(context.directory,'publication-trials.json',{observations,runs,recoveryRace,unchangedOriginals:true});
  const current=await currentPublication(context);
  return {current,heldout:{H05:'pass-source-actual-process-interruption-and-contention',H33:'pass-source-repeat-export-import'},
    observations,runs,liveCompetition,recoveryRace,finalNativeListeningJudgment:'pending-04-32/04-33',wholePack:'incomplete'};
}
async function currentPublication(context){
  const index=content.normalizeCurrent(ROOT),publicationRoot=path.join(context.directory,'current-publication'),before=snapshot(),controls=[];
  const baseline=await publish({sourceRoot:ROOT,index,publicationRoot,invocation:context.nonce+'-current'});
  need(baseline.sourceComplete&&baseline.outputs.length===180&&baseline.accepted===0&&!baseline.complete,'CURRENT_PUBLICATION_POSITIVE');
  const current=readPublished(publicationRoot),sourceRoot=inside(publicationRoot,current.pointer.generation),pointer=hash(bytes(publicationRoot,'current.json'));
  write(context.directory,'current-publication-positive.json',baseline);
  for(const step of ['locked','staged','validated','before-commit']){
    const invocation=context.nonce+'-current-'+step,name='current-interrupted-'+step;let caught;
    try{await runner.command(context,name,process.execPath,[path.join(ROOT,'src/godot-game/tools/asset-pipeline/content.cjs'),'--publish-worker',sourceRoot,publicationRoot,invocation,step],{interruptMarker:'PUBLICATION_PAUSED '+invocation+' '+step});}catch(error){caught=error;}
    const command=document(context.directory,name+'.command.json');need(caught?.message.includes('LOADER_INTERRUPTED')&&command.reason==='LOADER_INTERRUPTED'&&command.streamsClosed&&!command.cleanupError,'CURRENT_PUBLICATION_INTERRUPTION',step);
    need(hash(bytes(publicationRoot,'current.json'))===pointer&&readPublished(publicationRoot).result.valid.length===180,'CURRENT_PUBLICATION_VISIBLE_SIBLINGS',step);
    const recovered=content.recoverPublication({publicationRoot,invocation});need(!recovered.published,'CURRENT_INTERRUPTED_NOT_PUBLISHED');controls.push({id:name,pointerUnchanged:true,validSiblings:180,recovered});
  }
  const invalid=structuredClone(index);invalid.outputs[0].dependencies=[];
  let rejected;try{await publish({sourceRoot:ROOT,index:invalid,publicationRoot,invocation:context.nonce+'-current-invalid'});}catch(error){rejected=error;}
  need(rejected?.message.includes('PUBLICATION_INVALID_OUTPUT')&&hash(bytes(publicationRoot,'current.json'))===pointer,'CURRENT_INVALID_CANNOT_REPLACE');controls.push({id:'invalid-current-definition',rejected:true});
  const shuffled=structuredClone(index);shuffled.outputs.reverse();shuffled.files.reverse();
  const repeated=await publish({sourceRoot:ROOT,index:shuffled,publicationRoot,invocation:context.nonce+'-current-restored'});
  need(repeated.sourceComplete&&equal([...repeated.outputs].sort((a,b)=>a.id.localeCompare(b.id,'en')),[...baseline.outputs].sort((a,b)=>a.id.localeCompare(b.id,'en'))),'CURRENT_REPEAT_IDENTITIES');
  need(equal(snapshot(),before),'CURRENT_PUBLICATION_SOURCE_CHANGED');write(context.directory,'current-publication-controls.json',controls);
  return{sourceComplete:true,actualOutputs:180,accepted:0,complete:false,controls,restoredInvocation:repeated.invocation};
}
runner.registerCase('source.publication',{...definition,title:'Actual interrupted, competing and repeated publication',order:4,
  acceptance:['AC-02','AC-03','AC-16','AC-19','AC-22'],
  actions:['Publish actual tracked bow to a validated immutable generation','Interrupt owned worker at four observed stages',
    'Run competing separate process against its exclusive lock','Recover only dead exact invocation ownership',
    'Reject regressed, stale, missing-byte, reparse and escaped publications','Repeat export/import and compose explicit shared identities'],
  expected:'Four actual interrupted failed workers and actual locked competitor preserve current validated output; fresh repeated runs preserve one stable selected identity; no whole-pack completion',
  evidence:['inputs.json','interrupted-locked.command.json','interrupted-staged.command.json','interrupted-validated.command.json',
    'interrupted-before-commit.command.json','competitor.command.json','publication-trials.json','inputs-after.json','result.json']},publication);
module.exports={legacyFixture,snapshot,subject,failure,invalid,reference,sharedSubject,provenance,definition,publication,currentPublication};
