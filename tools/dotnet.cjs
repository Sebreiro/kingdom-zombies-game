'use strict';
const fs=require('node:fs'),path=require('node:path');

// SDK selection starts at the process working directory, even with an absolute project path.
function launch(root,executable,args){
  const workspace=path.join(root,'src/godot-game');
  if(!/^dotnet(?:\.exe)?$/i.test(path.basename(executable))||!fs.existsSync(path.join(workspace,'global.json')))
    return {cwd:root,args};
  return {cwd:workspace,args:args.map(arg=>typeof arg==='string'&&!path.isAbsolute(arg)
    &&(arg.includes('/')||arg.includes('\\')||/\.(?:slnx?|csproj|dll|cs|json)$/i.test(arg))&&fs.existsSync(path.join(root,arg))
    ?path.resolve(root,arg):arg)};
}
module.exports={launch};
