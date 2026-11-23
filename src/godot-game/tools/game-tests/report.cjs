'use strict';

const fs = require('node:fs');
const path = require('node:path');

function relative(root, file) {
  const name = path.relative(root, file);
  if (!name || path.isAbsolute(name) || name === '..' || name.startsWith('..' + path.sep)) {
    throw new Error('Evidence path is outside its owned directory: ' + file);
  }
  return name.replaceAll(path.sep, '/');
}

function requiredFile(root, file) {
  relative(root, file);
  relative(fs.realpathSync(root), fs.realpathSync(file));
  const stat = fs.statSync(file);
  if (!stat.isFile() || !stat.size) throw new Error('Empty or missing evidence: ' + file);
  return file;
}

function writeJson(directory, name, value) {
  const file = path.join(directory, name);
  fs.writeFileSync(file, JSON.stringify(value, null, 2) + '\n', {flag: 'wx'});
  return file;
}

function exact(expected, found) {
  if (!expected.length || expected.length !== found.length || new Set(expected).size !== expected.length
      || new Set(found).size !== found.length || found.some(id => !expected.includes(id))) {
    throw new Error('Result membership mismatch. Expected ' + expected.join(', ') + '. Received ' + found.join(', ') + '.');
  }
}

function reconcile(directory, selected, results) {
  exact(selected.map(row => row.id), results.map(row => row.id));
  for (const result of results) {
    if (!['pass', 'fail', 'blocked'].includes(result.status)) {
      result.message = 'Required check returned invalid status: ' + result.status;
      result.status = 'fail';
    }
    if (result.status !== 'pass') continue;
    try {
      if (!Array.isArray(result.artifacts) || result.artifacts.length === 0) throw new Error('Required evidence missing');
      for (const file of result.artifacts) requiredFile(directory, path.resolve(directory, file));
    } catch (error) {
      result.status = 'fail';
      result.message = error.message;
    }
  }
  return results;
}

module.exports = {exact, reconcile, relative, requiredFile, writeJson};
