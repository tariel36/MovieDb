import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { MediaItemsService } from '../../../media-items/services/media-items.service';
import { IMediaItem } from '../../../models/media/media-item.interface';
import { LocalStorageKeys } from '../../../utility/local-storage-keys.enum';
import { LocalStorageService } from '../../../utility/local-storage.service';

@Component({
  selector: 'app-library',
  templateUrl: './library.component.html',
  styleUrls: ['./library.component.scss']
})
export class LibraryComponent implements OnInit {

    public items: IMediaItem[] = []
    public isLoading: boolean = true;
    public hasItems: boolean = false;
    public itemsCountLabel: string = '';
    public searchText: string = '';

    constructor(
        private readonly mediaItemsService: MediaItemsService,
        private readonly localStorageService: LocalStorageService,
        private readonly translateService: TranslateService,
    ) { }

    public ngOnInit(): void {
        this.startLoading();
        
        const types = JSON.parse(this.localStorageService.getValue(LocalStorageKeys.selectedMediaItemTypes) ?? '[]') as number[];

        this.mediaItemsService.getAllMediaItems(types)
            .then(this.onItemsReceived.bind(this))
            .finally(this.onAfterItemsReceived.bind(this));
    }

    public onSearchKeyUp(args: KeyboardEvent): void {
        if (args.key === "Enter") {
            this.onSearchClick();
        }
    }

    public onSearchClick(): void {
        this.startLoading();
        
        const types = JSON.parse(this.localStorageService.getValue(LocalStorageKeys.selectedMediaItemTypes) ?? '[]') as number[];
        
        this.mediaItemsService.searchMediaItems(this.searchText, types)
            .then(this.onItemsReceived.bind(this))
            .finally(this.onAfterItemsReceived.bind(this));
    }

    public onSearchTextInput(args: any): void {
        this.searchText = (args?.target as any)?.value ?? '';
    }

    private onItemsReceived(items: IMediaItem[]): void {
        this.items = items;
    }

    private onAfterItemsReceived(): void {
        const items = (this.items ?? []);
        this.hasItems = items.length > 0;

        this.translateService.get('found-items-count', { count: items.length })
            .toPromise()
            .then(x => {
                this.itemsCountLabel = x;
                this.isLoading = false;
            })
    }

    private startLoading(): void {
        this.isLoading = true;
        this.items = [];
        this.hasItems = false;
    }
}
