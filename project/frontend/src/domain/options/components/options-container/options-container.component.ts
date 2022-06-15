import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { MatCheckboxChange } from '@angular/material/checkbox';
import { TranslateService } from '@ngx-translate/core';
import { ILanguage } from '../../../languages/models/language.interface';
import { LanguageSelectionService } from '../../../languages/services/language-selection.service';
import { MediaItemsService } from '../../../media-items/services/media-items.service';
import { MediaItemType } from '../../../models/media/media-item-type.enum';
import { Maybe } from '../../../models/utility/maybe.type';
import { LocalStorageKeys } from '../../../utility/local-storage-keys.enum';
import { LocalStorageService } from '../../../utility/local-storage.service';
import { IVisibleMediaItemType } from '../../models/visible-media-item-type.interface';

@Component({
  selector: 'app-options-container',
  templateUrl: './options-container.component.html',
  styleUrls: ['./options-container.component.scss']
})
export class OptionsContainerComponent
    implements OnInit
{
    public isLoading: boolean = true;

    public email: string = '';
    public languages: ILanguage[] = [];
    public items: IVisibleMediaItemType[] = [];

    private selectedLanguage: Maybe<ILanguage>;
    
    @Output() public settingsSaved: EventEmitter<void> = new EventEmitter<void>();

    constructor(
        private readonly languageSelectionService: LanguageSelectionService,
        private readonly mediaItemsService: MediaItemsService,
        private readonly localStorageService: LocalStorageService,
        private readonly translateService: TranslateService
    ) { }

    public ngOnInit(): void {
        // Get saved values
        const selectedMediaItemTypes = JSON.parse(this.localStorageService.getValue(LocalStorageKeys.selectedMediaItemTypes) ?? '[]') as number[];
        this.email = this.localStorageService.getValue(LocalStorageKeys.notificationEmail) ?? '';
        this.selectedLanguage = this.languageSelectionService.getLanguage();

        // Get data to display

        // Languages
        this.languages = this.languageSelectionService.languages;

        // Media types
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
        })
        .finally(() => {
            this.isLoading = false;
        });
    }

    public onEmailInput(args: any): void {
        this.email = (args?.target as any)?.value ?? '';
    }

    public isSelectedLanguage(lang: ILanguage): boolean {
        return this.selectedLanguage == lang;
    }

    public onFlagClick(lang: ILanguage): void {
        this.selectedLanguage = lang;
    }

    public onItemChanged(args: MatCheckboxChange, item: IVisibleMediaItemType): void {
        item.checked = args.checked;
    }

    public onApplyClicked(): void {
        this.isLoading = true;

        // Save language
        this.localStorageService.setValue(LocalStorageKeys.notificationEmail, this.email);
        this.localStorageService.setValue(LocalStorageKeys.isNotificationSelected, "1");

        // Save types
        const selected = this.items.filter(x => x.checked).map(x => x.id);
        this.localStorageService.setValue(LocalStorageKeys.selectedMediaItemTypes, JSON.stringify(selected));
        this.localStorageService.setValue(LocalStorageKeys.isMediaItemTypesSelected, "1");

        // Save language
        if (this.selectedLanguage != null) {
            this.languageSelectionService.setLanguage(this.selectedLanguage);
        }
        
        // Save in backend
        this.mediaItemsService.setNotificationEmail(this.email, selected)
            .finally(() => { 
                this.onSettingsSaved();
                window.location.reload();
            });
    }

    private onSettingsSaved(): void {
        if (this.settingsSaved == null) {
            return;
        }

        this.settingsSaved.emit();
    }
}
