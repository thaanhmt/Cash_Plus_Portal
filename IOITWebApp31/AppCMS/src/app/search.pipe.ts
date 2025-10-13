import { Pipe, PipeTransform } from '@angular/core';
@Pipe({
  name: 'itemTitleFilter'
})
export class SearchPipe implements PipeTransform {
  transform(value: any, args?: any): any {
    if (!args) {
      return value;
    }
    return value.filter((val) => {
      let rVal = (val.Title.includes(args));
      return rVal;
    })

  }
}
