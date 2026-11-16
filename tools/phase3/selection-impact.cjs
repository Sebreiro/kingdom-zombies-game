'use strict';
// Phase 3's explicit dependency assessment. This is not a general Phase 5 impact selector.
const sources=['tools/code-rules/BodyLengthAnalyzer.cs','tools/code-rules/EngineGeneratedSyntax.cs',
  'docs/code/register.json','docs/code/engine-generated-spans.json','docs/code/generated-callsite-adaptations.json',
  'docs/code/exemptions.json','docs/code/visibility-exemptions.json','src/godot-game/tests/verification/KingdomZombies.Phase1.Tests.csproj',
  'src/godot-game/tests/verification/BuildFixture.cs','src/godot-game/tests/verification/ExemptionValidator.cs','src/godot-game/tests/verification/ExemptionTests.cs'];
const consumers=['BodyLengthAnalyzerTests','ContinuationIndentAnalyzerTests','LineLengthAnalyzerTests',
  'ThisQualificationAnalyzerTests','NamedArgumentAnalyzerTests','MemberOrderAnalyzerTests'];
function affected(cases){
  return cases.filter(row=>(row.acceptancePhase??1)<=1&&(row.group==='register'||/^rules\.body/.test(row.id)||/^R-37\./.test(row.id)||
    consumers.some(consumer=>row.test?.includes('.'+consumer+'.'))||/^rules\.(analyzer-|generated-|godot-|suppression-)/.test(row.id)||
    /^edge\.rule-0[23]\./.test(row.id)||/^R-(46|50|54|67)\./.test(row.id)||
    row.test?.includes('.ExemptionTests.')||row.test?.includes('.SuppressionTests.')||['ac-05.complete','ac-08.complete'].includes(row.id))).map(row=>row.id);
}
function check(selection,cases,requireTrue){
  const required=affected(cases);
  for(const id of required)requireTrue(selection.selected.phase1.includes(id),'AFFECTED_PHASE1_OMITTED '+id);
  requireTrue(Array.isArray(selection.impactSources)&&sources.every(source=>selection.impactSources.includes(source)),'IMPACT_SOURCE_ASSESSMENT');
  requireTrue(!selection.omitted.phase1.some(row=>/no edits to analyzer implementations/i.test(row.reason)),'FALSE_ANALYZER_OMISSION_REASON');
  return required;
}
module.exports={sources,affected,check};
