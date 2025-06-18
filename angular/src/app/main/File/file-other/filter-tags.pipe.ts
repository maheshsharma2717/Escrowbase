import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'filterTags', // ✅ Ensure the name matches the template usage
  pure: false // ✅ Allows dynamic updates when `tagFilter` changes
})
export class FilterTagsPipe implements PipeTransform {
  transform(files: any[], tagFilter: string): any[] {
    if (!tagFilter) return files; // ✅ Return all if no filter

    return files.filter(file =>
      file.escrowFileTags.some(tag =>
        tag.tagDescription.toLowerCase().includes(tagFilter.toLowerCase())
      )
    );
  }
}
