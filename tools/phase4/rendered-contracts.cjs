'use strict';

const {isDeepStrictEqual:equal}=require('node:util');
const {need,exact,named}=require('./evidence.cjs');

const HUB='/root/Inspection', WORKSPACE=HUB+'/Shell/Workspace', PREVIEW=WORKSPACE+'/Preview';
const BROWSER=WORKSPACE+'/Controls/Browser', CURRENT=PREVIEW+'/Surface/Current', SUMMARY=HUB+'/Summary';
const TOOLBAR=HUB+'/Shell/Toolbar', CATEGORY=WORKSPACE+'/Navigation/Category';
const SEARCH=BROWSER+'/Search';
const METADATA=PREVIEW+'/Metadata', FAILURES=PREVIEW+'/Failures', CHAR=CURRENT+'/Character';
const WORLD=CURRENT+'/WorldItems', RESOURCE=CURRENT+'/ResourcePreview';
const DATA=CURRENT+'/DataText/Body', AUDIO=CURRENT+'/AudioPreview/Audio/Content';
const RESOURCES=BROWSER+'/Resources', SELECT=id=>CHAR+'/Controls/Slots/'+id+'/Select';
const ITEM_SELECT=WORLD+'/InspectorScroll/Body/Art/ItemsWeaponsPreview/Selection';
const DEFENSE=WORLD+'/InspectorScroll/Body/Art/Content';
const WORLD_STATE=WORLD+'/InspectorScroll/Body/Choices/CurrentState';
const WORLD_OWNER=DEFENSE+'/WorldPreview', WORLD_CHOICES=WORLD+'/InspectorScroll/Body/Choices';
const WORLD_STEP=WORLD_CHOICES+'/Step', CHAR_STEP=CHAR+'/Controls/Slots/Step';
const READY='Validated; visual/listening acceptance pending';
const PARTIAL='Partial — validated bytes; native/visual/listening acceptance pending';
const eq=(node,property,value)=>({node,property,match:'equal',value});
const contains=(node,property,value)=>({node,property,match:'contains',value});
const field=(node,property,key,value)=>({node,property,match:'field',key,value});
const child=(childClass,childIndex,property,value)=>({...eq(WORLD_OWNER,property,value),childClass,childIndex});
const childCount=(childClass,value)=>({node:WORLD_OWNER,property:null,match:'child-count',childClass,value});
const exists=node=>({node,property:null,match:'present'});
const absent=node=>({node,property:null,match:'absent'});
const label=(node,text)=>eq(node,'text',text);
const subject=id=>label(SUMMARY+'/ResourceId',id);
const show=node=>eq(node,'visible',true);
const stage=(id,control,checks)=>({id,control,checks});
const choice=(id,control,selected,checks=[])=>({...stage(id,control,[eq(control,'selected',selected),...checks]),openMenu:true});
const open=(id,resource,checks)=>stage(id,RESOURCES,[subject(resource),...checks]);
const view=(name,id,checks)=>open(name,id,[field(RESOURCE,'metadata/resource_view','id',id),...checks]);
const readyChecks=[label(TOOLBAR+'/Operation',READY),eq(TOOLBAR+'/Cancel','disabled',true),show(TOOLBAR+'/Operation')];
const baseline=[eq(CATEGORY,'selected',1),label(SEARCH,''),eq(BROWSER+'/Status','selected',0),
  subject('IT-07'),eq(METADATA,'visible',false),...readyChecks.slice(0,2)];
const itemBody=(id)=>[show(WORLD),eq(ITEM_SELECT,'assigned_animation',id)];
const missing=[
  subject('IT-11'),
  label(SUMMARY+'/Status','Error — IT-11 — WORLD_INSPECTOR_MISSING content/packs/items-11-24.json; choose another resource or Recheck'),
  contains(FAILURES,'text','IT-11: res://content/pack-index.json, field outputs, value '),
  contains(FAILURES,'text','actual named completed definition unavailable'),
  contains(FAILURES,'text','res://content/packs/items-11-24.json, field bytes, value read — missing or oversized resource'),
  show(FAILURES)
];
const missingCase=[
  open('missing-resource','IT-11',missing.slice(1)),
  open('valid-sibling','IT-07',[label(SUMMARY+'/ResourceName','Authored world/item view · IT-07'),
    label(SUMMARY+'/Status',PARTIAL),...itemBody('IT-07')])
];
const cleanPackage=[
  open('reachable-first','IT-07',[...itemBody('IT-07'),...readyChecks.slice(0,2)]),
  open('reachable-second','IT-07',[...itemBody('IT-07'),...readyChecks.slice(0,2)])
];
const character=field(CHAR,'metadata/sample','action','bow');
const detail=(id,resource,shared)=>stage(id,PREVIEW+'/Actions/Details',[subject(resource),show(METADATA),
  contains(METADATA,'text','"id": "'+resource+'"'),contains(METADATA,'text','"sharedFileId": "'+shared+'"'),
  contains(METADATA,'text','Explicit shared mappings: ')]);
const resourceState=(name,value)=>field(RESOURCE,'metadata/resource_view',name,value);
const cancel=(id,checks)=>({...stage(id,TOOLBAR+'/Cancel',checks),recheck:true});
const AUDIO_CASE='rendered.audio-audition', AUDITION_PROPERTY='metadata/audition';
const SWING='melee.swing.fist-knife-machete', IMPACT='melee.impact.flesh', CONFIRM='ui.action.confirm';
const AUDIO_PLAYBACK=new Map([
  ['play-swing',{route:SWING,state:'quiet'}],
  ['play-day-swing',{route:SWING,state:'day-fire'}],
  ['play-night-swing',{route:SWING,state:'night-wind'}],
  ['play-impact',{route:IMPACT,state:'quiet'}],
  ['play-confirmation',{route:CONFIRM,state:'quiet'}],
  ['play-day-confirmation',{route:CONFIRM,state:'day-fire'}],
  ['play-night-confirmation',{route:CONFIRM,state:'night-wind'}]
]);
const AUDIO_DECISIONS=new Map([
  ['D-31',['play-swing','play-impact','play-confirmation']],
  ['D-32',['play-day-swing','play-night-swing','play-day-confirmation','play-night-confirmation']]
]);
const audioMenu=(id,control,index,checks=[])=>choice(id,AUDIO+'/'+control,index,checks);
const audition=id=>stage(id,AUDIO+'/Play',[{node:AUDIO,property:AUDITION_PROPERTY,match:'audition',value:AUDIO_PLAYBACK.get(id).route}]);
const dayFire='ambience.day-night.forest-day,ambience.fire-wind.local-fire';
const nightWind='ambience.day-night.forest-night,ambience.fire-wind.wind';
const restoreAudio=(id,index,loops)=>audioMenu(id,'CurrentState',index,[eq(AUDIO,'metadata/loops',loops)]);
const stopAudio=id=>stage(id,AUDIO+'/Stopall',[eq(AUDIO,'metadata/players',0),eq(AUDIO,'metadata/loops','')]);
function gear(slot,part,id) {
  const node=CHAR+'/Art/'+slot+'_'+part;
  return [show(node),eq(node,'metadata/item',id),eq(node,'metadata/slot',slot)];
}
const worldMeta=(key,value)=>field(WORLD,'metadata/library_view',key,value);
const terrain=[show(WORLD_OWNER),child('Sprite2D',20,'metadata/resource_id','ground_20'),child('Sprite2D',20,'visible',true)];
const ruin=[worldMeta('id','BL-16'),worldMeta('state','portal.ruins'),eq(WORLD_OWNER,'metadata/resource_id','portal.ruins'),
  child('Sprite2D',275,'metadata/resource_id','left.portal.1.ruin.0'),child('Sprite2D',275,'metadata/permanent',true),
  child('Sprite2D',275,'visible',true)];
const death=(time)=>[subject('FX-08'),worldMeta('id','FX-08'),worldMeta('state','effect:character.death'),
  worldMeta('eventCount',1),worldMeta('time',time),childCount('CPUParticles2D',time<0.25?1:0),
  ...(time<0.25?[child('CPUParticles2D',0,'metadata/resource_id','effect.character.death'),
    child('CPUParticles2D',0,'metadata/preview_time',time),child('CPUParticles2D',0,'visible',true)]:[])];
const walk=(pose,stride)=>[subject('CH-01'),field(CHAR,'metadata/sample','movement','walk'),
  field(CHAR,'metadata/sample','lowerPose',pose),field(CHAR,'metadata/sample','strideSeconds',stride),
  show(CHAR+'/Art/Body_lower'),...gear('right','held','IT-03')];
const P4_DEATH_CHECKPOINTS=['death-step','death-retired'];

const CASES = new Map([
  ['heldout.H01',[
    detail('owner-first','IT-07','shared.file.6d61b47b9b52574a102f'),
    detail('owner-second','IT-08','shared.file.6d61b47b9b52574a102f')
  ]],
  ['heldout.H12',[
    choice('stage-bow',SELECT('Ammunition'),2,[subject('AN-08'),character]),
    choice('empty-arrows',SELECT('Ammunition'),0,[subject('AN-08'),character,field(CHAR,'metadata/sample','nocked',false)]),
    choice('last-arrow',SELECT('Ammunition'),1,[subject('AN-08'),character,field(CHAR,'metadata/sample','nocked',true)]),
    choice('broken',SELECT('Condition'),1,[subject('AN-08'),field(CHAR,'metadata/sample','nocked',false)]),
    choice('usable',SELECT('Condition'),0,[subject('AN-08'),character]),
    ...[1,2,3].flatMap(count=>[
      open('tower-'+count,'BL-0'+(6+count),[show(DEFENSE+'/Status')]),
      choice('ready-guards-'+count,WORLD_STATE,8,[eq(DEFENSE,'metadata/functional_tier',count),
        eq(DEFENSE,'metadata/guard_count',count),show(DEFENSE+'/Status'),contains(DEFENSE+'/Status','text','guards '+count)])
    ]),
    view('persistent-corpse','CH-13',[resourceState('controller','CorpsePreview'),resourceState('state','persistent corpse'),
      exists(RESOURCE+'/Art/Corpse/Visual')]),
    stage('corpse-time-boundary',RESOURCE+'/Controls/Step',[resourceState('state','persistent corpse'),
      resourceState('time',0.125),exists(RESOURCE+'/Art/Corpse/Visual')]),
    open('death-gallery','FX-08',[worldMeta('id','FX-08'),show(WORLD_OWNER)]),
    choice('death-effect',WORLD_STATE,1,death(0)),
    stage('death-step',WORLD_STEP,death(0.125)),
    stage('death-retired',WORLD_STEP,death(0.25))
  ]],
  ['heldout.H13',[
    choice('coat',SELECT('torso'),2,[subject('CH-01'),...gear('torso','torso-front','IT-13')]),
    choice('armor',SELECT('armor'),2,[...gear('armor','torso-front','IT-20')]),
    choice('belt',SELECT('belt'),1,[...gear('belt','belt-front','IT-21')]),
    choice('bag',SELECT('backpack'),2,[...gear('backpack','bag-body','IT-24')]),
    choice('overlap',SELECT('right'),6,[subject('CH-01'),...gear('right','held','IT-07'),
      ...gear('torso','torso-front','IT-13'),...gear('armor','torso-front','IT-20'),
      ...gear('belt','belt-front','IT-21'),...gear('backpack','bag-body','IT-24')]),
    view('nearby-bodies','CH-12',[resourceState('controller','CrowdPreview'),exists(RESOURCE+'/Art/Crowd/survivor-001')]),
    choice('nearby-target',RESOURCE+'/Controls/State',1,[resourceState('state','survivor-001'),
      exists(RESOURCE+'/Art/Crowd/survivor-001/Visual/SelectedMarker'),show(RESOURCE+'/Controls/Identity')])
  ]],
  ['heldout.H15',[
    choice('facing',SELECT('Facing'),1,[subject('AN-07'),eq(CHAR,'metadata/facing',-1)]),
    choice('melee',SELECT('Action'),1,[subject('AN-07'),field(CHAR,'metadata/sample','action','melee'),eq(CHAR,'metadata/facing',-1)]),
    choice('action',SELECT('Action'),2,[subject('AN-07'),character,eq(CHAR,'metadata/facing',-1)]),
    choice('gear-away',SELECT('left'),2,[eq(CHAR,'metadata/facing',-1),...gear('left','held','IT-03')]),
    choice('gear-order',SELECT('left'),6,[subject('AN-07'),eq(CHAR,'metadata/facing',-1),...gear('left','held','IT-07'),
      eq(CHAR+'/Art/left_held','metadata/logical_hand','left'),field(CHAR,'metadata/sample','bowHand','left'),show(CHAR+'/Status')])
  ]],
  ['heldout.H21',[
    choice('search-data-category',CATEGORY,4,[show(RESOURCES)]),
    {...stage('zero-results',SEARCH,[label(SEARCH,'q'),
      label(BROWSER+'/Count','0 results — No resources match. Clear the filter or choose another category.')]),
      key:81,focusClick:true},
    {...stage('clear-search',SEARCH,[label(SEARCH,'')]),key:8},
    choice('invalid-reference',DATA+'/Choices/State',2,[subject('DT-24'),show(DATA+'/Detail'),
      contains(DATA+'/Detail','text','Missing definition TXT-unknown: content/text/en.json, field /rows/TXT-unknown.')])
  ]],
  ['UIH.HUB-NAV.error',[
    cancel('cancel-error',[label(TOOLBAR+'/Operation','Cancelled; no earlier validation reused'),
      label(SUMMARY+'/Status','Unavailable — validation cancelled; choose Recheck resources'),show(TOOLBAR+'/Recheck')]),
    stage('retry',TOOLBAR+'/Recheck',readyChecks)
  ]],
  ['UIH.HUB-REGISTER.partial',[
    choice('register',CATEGORY,6,[contains(SUMMARY+'/PackStatus','text','180/180 source-ready · 0/180 accepted'),
      show(RESOURCES),show(SUMMARY+'/PackStatus')])
  ]],
  ['UIH.CHARACTER-PREVIEW.populated',[
    open('character','CH-01',[eq(CHAR,'metadata/body','CH-01'),show(CHAR+'/Status'),show(CHAR+'/Art/Body_upper')]),
    choice('equip-walking-character',SELECT('right'),2,[subject('CH-01'),...gear('right','held','IT-03')]),
    choice('start-walking',SELECT('Movement'),2,walk('walk-near',0)),
    stage('walk-far',CHAR_STEP,walk('walk-far',0.12)),
    stage('walk-near',CHAR_STEP,walk('walk-near',0))
  ]],
  ['UIH.WORLD-ITEM-PREVIEW.populated',[
    open('world-item','IT-07',[...itemBody('IT-07'),show(SUMMARY+'/ResourceName')]),
    open('assembled-world','world.assembled',terrain),
    choice('assembled-world-day',WORLD_STATE,22,[subject('world.assembled'),eq(WORLD_OWNER,'metadata/resource_id','world.day'),...terrain]),
    open('portal-library','BL-16',[worldMeta('id','BL-16'),show(WORLD_OWNER)]),
    choice('portal-ruins',WORLD_STATE,6,ruin),
    choice('portal-ruins-focus',WORLD_CHOICES+'/Portal',1,[eq(WORLD_CHOICES+'/Pan','value',-800),...ruin])
  ]],
  ['UIH.UI-TEXT-STATUS.long-text',[
    choice('expanded-text',DATA+'/Choices/TextLength',1,[subject('TXT-02'),show(DATA+'/Detail')]),
    choice('long-text',DATA+'/Choices/State',7,[subject('TXT-02'),show(DATA+'/Detail'),
      eq(DATA+'/Detail','wrap_mode',1),contains(DATA+'/Detail','text','Élodie · Jérôme · İpek · Zoë')])
  ]],
  ['UIH.AUDIO-AUDITION.populated',[
    open('audio','AU-01',[show(AUDIO+'/Event'),show(AUDIO+'/Details'),contains(AUDIO+'/Details','text','Source ')])
  ]],
  ['UIH.UI-FIXTURE-PREVIEW.populated',[
    view('ui-fixture','UI-01',[resourceState('controller','Source-bound UI presenter'),
      show(RESOURCE+'/Controls/Identity'),exists(RESOURCE+'/Art/LibraryScroll/Surface')])
  ]],
  ['rendered.hub-navigation',[
    ...[0,1,2,3,4,5].map((index)=>choice('category-'+index,CATEGORY,index,[show(RESOURCES)])),
    detail('details','AU-01','shared.file.c0761264b172c09e69c9'),
    stage('dismiss-details',PREVIEW+'/Actions/Details',[eq(METADATA,'visible',false),label(PREVIEW+'/Actions/Details','Show details')]),
    cancel('visible-error',[label(SUMMARY+'/Status','Unavailable — validation cancelled; choose Recheck resources')]),
    stage('retry-after-error',TOOLBAR+'/Recheck',readyChecks)
  ]],
  [AUDIO_CASE,[
    choice('enter-audio',CATEGORY,5,[show(RESOURCES)]),
    open('cue','AU-03',[show(AUDIO+'/Details'),contains(AUDIO+'/Details','text','Source ')]),
    audioMenu('select-swing','Event',2),
    audition('play-swing'),
    restoreAudio('restore-loop',1,dayFire),audition('play-day-swing'),
    restoreAudio('restore-night-swing',2,nightWind),audition('play-night-swing'),stopAudio('stop-audio'),
    open('cue-impact','AU-04',[show(AUDIO+'/Status')]),audioMenu('select-impact','Event',2),audition('play-impact'),
    open('cue-confirmation','AU-26',[show(AUDIO+'/Status')]),audioMenu('select-confirmation','Event',1),
    audition('play-confirmation'),
    restoreAudio('restore-day-confirmation',1,dayFire),audition('play-day-confirmation'),
    restoreAudio('restore-night-confirmation',2,nightWind),audition('play-night-confirmation'),stopAudio('stop-confirmation'),
    choice('return-items',CATEGORY,1,[show(RESOURCES)])
  ]],
  ['heldout.H27',missingCase],
  ['heldout.H28',missingCase],
  ['heldout.H31',cleanPackage],
  ['rendered.packaged-relaunch',cleanPackage]
]);

function matches(check,value) {
  if(check.match==='present')return value===true;
  if(check.match==='absent')return value===false;
  if(check.match==='equal'||check.match==='child-count')return equal(value,check.value);
  if(check.match==='contains')return typeof value==='string'&&value.includes(check.value);
  if(check.match==='audition'&&typeof value==='string') {
    try{return JSON.parse(value)?.cue?.route===check.value;}catch{return false;}
  }
  if(check.match==='field'&&typeof value==='string') {
    try{return equal(JSON.parse(value)?.[check.key],check.value);}catch{return false;}
  }
  return false;
}

function template(id) {
  const required=CASES.get(id);
  need(required,'RENDERED_CASE_CONTRACT',id);
  const repeated=id==='heldout.H31'||id==='rendered.packaged-relaunch';
  const reset=lifetime=>({...stage('reset-'+lifetime,RESOURCES,baseline),lifetime,reset:true});
  if (!repeated) return [...required.map(row=>({...row,lifetime:1})),reset(1)];
  return [{...required[0],lifetime:1},reset(1),{...required[1],lifetime:2},reset(2)];
}

const NAVIGATION = new Set([CATEGORY,RESOURCES,SEARCH,BROWSER+'/Status',TOOLBAR+'/Recheck',METADATA,PREVIEW+'/Actions/Details']);
const NAVIGATION_MENUS = new Map([[CATEGORY,8],[BROWSER+'/Status',4]]);
const NAVIGATION_SCROLLS = new Set([CHAR+'/Controls',WORLD+'/InspectorScroll',WORLD+'/InspectorScroll/Body/Art']);

function validateDeclaration(driver,item) {
  const required=template(item.id);
  need(driver.id===item.id&&named(driver.stimulus)&&Array.isArray(driver.steps)&&driver.steps.length<=64,
    'RENDERED_STIMULUS',item.id);
  exact(driver.steps.map(step=>step.id),driver.steps.map(step=>step.id),'physical checkpoint IDs');
  exact(required.map(step=>step.id),driver.steps.filter(step=>!step.navigate).map(step=>step.id),item.id+' required checkpoints',true);
  let next=0;
  for(const step of driver.steps) {
    need(/^[a-z][a-z0-9-]{0,47}$/.test(step.id)&&(step.lifetime===1||step.lifetime===2),'RENDERED_CHECKPOINT',item.id);
    const coordinates=point=>Number.isInteger(point?.x)&&point.x>=0&&point.x<1280&&Number.isInteger(point?.y)&&point.y>=0&&point.y<720;
    const click=step.action==='click'&&coordinates(step);
    const tap=step.action==='tap'&&Number.isInteger(step.key)&&step.key>0&&step.key<256&&step.key!==9;
    need(click||tap,'RENDERED_CASE_ACTION',item.id+' '+step.id);
    const expected=step.navigate?null:required[next];
    const focusRequired=expected?.focusClick===true;
    if (focusRequired || step.focusClick!==undefined) {
      need((focusRequired||step.navigate)&&step.control===SEARCH&&tap&&coordinates(step.focusClick),
        'RENDERED_SEARCH_FOCUS',item.id+' '+step.id);
    }
    const optionCount=step.navigate?NAVIGATION_MENUS.get(step.control):undefined;
    const menuRequired=step.navigate?optionCount!==undefined:expected.openMenu===true;
    need(menuRequired ? click&&coordinates(step.openMenu)&&!equal([step.x,step.y],[step.openMenu.x,step.openMenu.y]) : !step.openMenu,
      'RENDERED_CASE_MENU',item.id+' '+step.id);
    if(step.navigate) {
      const scroll=NAVIGATION_SCROLLS.has(step.control);
      need(!step.recheck&&step.id.startsWith('navigate-')&&required[next]?.lifetime===step.lifetime&&(NAVIGATION.has(step.control)||scroll)&&
        Array.isArray(step.checks)&&step.checks.length===1&&
        (step.checks[0].node===step.control || step.control===RESOURCES&&step.checks[0].node===SUMMARY+'/ResourceId')&&
        (scroll?step.checks[0].property==='scroll_vertical'&&Number.isInteger(step.checks[0].value)&&step.checks[0].value>=0:
          ['text','selected','visible'].includes(step.checks[0].property))&&step.checks[0].match==='equal'&&
        ['string','number','boolean'].includes(typeof step.checks[0].value),'RENDERED_NAVIGATION_CHECKPOINT',step.id);
      if (menuRequired) {
        const {property,value}=step.checks[0];
        need(property==='selected'&&Number.isInteger(value)&&value>=0&&value<optionCount,'RENDERED_NAVIGATION_MENU',step.id);
      }
    } else {
      next++;
      need(step.control===expected.control&&step.lifetime===expected.lifetime&&equal(step.checks,expected.checks),
        'RENDERED_CASE_OBSERVATIONS',item.id+' '+step.id);
      need(expected.key===undefined||tap&&step.key===expected.key,'RENDERED_CASE_KEY',item.id+' '+step.id);
      need(expected.recheck ? click&&coordinates(step.recheck)&&!equal([step.x,step.y],[step.recheck.x,step.recheck.y]) : !step.recheck,
        'RENDERED_CASE_RECHECK',item.id+' '+step.id);
    }
  }
  return driver;
}

module.exports={CASES,template,validateDeclaration,matches,readyChecks,baseline,P4_DEATH_CHECKPOINTS,
  AUDIO_CASE,AUDITION_PROPERTY,AUDIO_PLAYBACK,AUDIO_DECISIONS};
