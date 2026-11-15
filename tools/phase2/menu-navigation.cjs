'use strict';
async function check(send,observe,requireTrue){
  for(const control of ['Host','Join','Quit','Profiles','Name','NewProfile','Start']){
    await send('tap','Tab');requireTrue((await observe('tab-'+control.toLowerCase())).focusedControl===control,'TAB_FOCUS '+control);
  }
  await send('down','Shift_L');
  try{
    for(const control of ['NewProfile','Name','Profiles','Quit','Join','Host','Start']){
      await send('tap','Tab');requireTrue((await observe('shift-tab-'+control.toLowerCase())).focusedControl===control,'SHIFT_TAB_FOCUS '+control);
    }
  }finally{await send('up','Shift_L');}
  for(const control of ['Host','Join','Quit']){
    await send('tap','Down');requireTrue((await observe('down-'+control.toLowerCase())).focusedControl===control,'DOWN_FOCUS '+control);
  }
  for(const control of ['Join','Host','Start']){
    await send('tap','Up');requireTrue((await observe('up-'+control.toLowerCase())).focusedControl===control,'UP_FOCUS '+control);
  }
}
module.exports={check};
