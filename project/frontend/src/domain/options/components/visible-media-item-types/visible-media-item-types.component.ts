import { Component, OnInit } from '@angular/core';
import { MatCheckboxChange } from '@angular/material/checkbox';
import { TranslateService } from '@ngx-translate/core';
import { MediaItemsService } from '../../../media-items/services/media-items.service';
import { MediaItemType } from '../../../models/media/media-item-type.enum';
import { Maybe } from '../../../models/utility/maybe.type';
import { LocalStorageKeys } from '../../../utility/local-storage-keys.enum';
import { LocalStorageService } from '../../../utility/local-storage.service';
import { IVisibleMediaItemType } from '../../models/visible-media-item-type.interface';

@Component({
  selector: 'app-visible-media-item-types',
  templateUrl: './visible-media-item-types.component.html',
  styleUrls: ['./visible-media-item-types.component.scss']
})
export class VisibleMediaItemTypesComponent
    implements OnInit
{

    public items: IVisibleMediaItemType[] = [];

    constructor(
        private readonly mediaItemsService: MediaItemsService,
        private readonly localStorageService: LocalStorageService,
        private readonly translateService: TranslateService
    ) { }
  
    public ngOnInit(): void {
      const selectedMediaItemTypes = JSON.parse(this.localStorageService.getValue(LocalStorageKeys.selectedMediaItemTypes) ?? '[]') as number[];
  
      const createItem = (type: MediaItemType, label: string) => {
          return { checked: selectedMediaItemTypes.includes(type), id: type, label: this.translateService.instant(label) };
      };
  
      this.translateService.get([
          'media-item-type-unknown',
          'media-item-type-anime',
          'media-item-type-cartoon',
          'media-item-type-movie',
          'media-item-type-series'
      ])
      .toPromise()
      .then((labels: any) => {
          this.items = [
              createItem(MediaItemType.Unknown, labels['media-item-type-unknown']),
              createItem(MediaItemType.Anime, labels['media-item-type-anime']),
              createItem(MediaItemType.Cartoon, labels['media-item-type-cartoon']),
              createItem(MediaItemType.Movie, labels['media-item-type-movie']),
              createItem(MediaItemType.Series, labels['media-item-type-series']),
          ];
      });
    }
  
    public onApplyClicked(): void {
      const selected = this.items.filter(x => x.checked).map(x => x.id);
      this.localStorageService.setValue(LocalStorageKeys.selectedMediaItemTypes, JSON.stringify(selected));
      this.localStorageService.setValue(LocalStorageKeys.isMediaItemTypesSelected, "1");

      const email: Maybe<string> = this.localStorageService.getValue(LocalStorageKeys.notificationEmail);

      if (email != null) {
        this.mediaItemsService.setNotificationEmail(email, selected)
            .finally(() => { window.location.reload(); });
      } else {
        window.location.reload();
      }
    }
  
    public onItemChanged(args: MatCheckboxChange, item: IVisibleMediaItemType): void {
      item.checked = args.checked;
    }
}
