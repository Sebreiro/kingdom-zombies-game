'use strict';

const path=require('node:path');
const {registerCase}=require('../check.cjs');
const tool=require('../export.cjs');
const {need,write}=require('../content.cjs');

async function hubConsumer(context){
  const built=await tool.buildPackage(context);
  const {clean,engine,contract}=built;
  const pck=path.join(clean,'KingdomZombies.pck');
  write(context.directory,'hub-source-contract.json',{
    ...contract,index:undefined,expected:undefined,tracer:undefined,
    expectedObligations:contract.index.expectedObligations,
    independentIds:contract.expected.obligations.map(row=>row.id).sort(),
    retainedBow:{id:contract.tracer.id,name:contract.tracer.name,path:contract.tracer.path,
      sha256:contract.tracer.sha256,image:contract.tracer.image,registration:contract.tracer.registration},
  });
  const {result,controls,runtimeResolution}=await tool.portable(context,'hub-native',clean,contract);
  write(context.directory,'observer-controls.json',controls);
  const resources=await tool.packagedProbe(context,'hub-package-resource',engine,pck,contract);
  tool.verifyInputs(built);
  need(result.packageSha256===resources.pckSha256,'HUB_PACKAGE_IDENTITY');
  const evidence={portable:clean,files:built.closure.rows,native:result,runtimeResolution,
    editorSupport:resources,observerControls:controls.length};
  write(context.directory,'hub-package.json',evidence);
  return evidence;
}

registerCase('export.hub-consumer',{
  title:'Current packaged inspection hub and consumer',
  rationale:'The actual packaged hub must independently load and diagnose current content before acceptance',
  group:'export',importance:'Critical',needsGameWindow:false,runGroup:'G0-export-hub',order:310,
  requirements:['ASSET-05'],acceptance:['AC-15','AC-16','AC-17'],
  setup:'Pinned official Windows debug export template, production scene and C# projects, runtime source and independent obligation register',
  actions:['Publish and export the current production project','Check runtime and PCK member closures',
    'Start the exported executable naturally with only read-only debugger observations',
    'Verify exact hub paths, resource and register bindings, and independent rejection controls',
    'Check the same exported PCK and assemblies with the separate editor-mounted resource probe'],
  expected:'Clean exported headless exit with exact current hub, selected bow binding, register and controls; edited records fail independently',
  reset:'Use fresh owned package and profile directories and a bounded loopback observer; retain child and packet receipts',
  evidence:['project-inputs.json','pck-runtime-closure.json','hub-source-contract.json','hub-native.command.json',
    'hub-native-debugger.json','hub-native-observation.json','observer-controls.json','hub-package-resource.command.json',
    'hub-package.json','result.json']
},hubConsumer);
