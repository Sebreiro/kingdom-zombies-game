'use strict';
const runner=require('../check.cjs'),tool=require('../export.cjs');
runner.registerCase('export.feasibility',{group:'export',title:'Genuine Windows mono tracer export bundles net10 runtime',importance:'Critical',
  rationale:'Authoring-path or shared-runtime dependence blocks portable Windows delivery',needsGameWindow:false,runGroup:'G0',order:5,
  requirements:['ASSET-01','ASSET-05'],acceptance:['AC-15','AC-17'],status:'required',tags:['source-only','native-export','portable-runtime','PCK'],
  dependencies:['export.layout','export.profile','export.profile-boundaries','source.tracer'],
  setup:'Authenticated official 4.7.2 mono templates, pinned editor/SDK and real existing Godot project',
  actions:['Authenticate selected template archive and exact Windows members','Publish actual existing managed projects self-contained',
    'Export real tracer/resources through matching template','Inspect complete runtime dependency closure','Run clean exported application headlessly with development/runtime overrides removed'],
  expected:'Genuine template binary/PCK, complete net10 runtime/deps and actual packaged inspection entry; foundation startup/editor-mounted PCK are supporting evidence only',
  reset:'Fresh owned source/export/portable directories and bounded descendant cleanup; preserve all attempts',
  evidence:['inputs.json','project-inputs.json','templates.json','managed-publish.command.json','native-export.command.json','closure.json',
    'portable-headless.command.json','portable-headless-host.log','portable-headless-debugger.json','portable-headless-observation.json',
    'observer-controls.json','packaged-resource.command.json','negative-controls.json','result.json']},tool.exportTracer);
