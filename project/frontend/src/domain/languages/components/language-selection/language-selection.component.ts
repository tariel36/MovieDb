import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { LocalStorageKeys } from '../../../utility/local-storage-keys.enum';
import { LocalStorageService } from '../../../utility/local-storage.service';
import { ILanguage } from '../../models/language.interface';

@Component({
  selector: 'app-language-selection',
  templateUrl: './language-selection.component.html',
  styleUrls: ['./language-selection.component.scss']
})
export class LanguageSelectionComponent implements OnInit {

    @Input() public isLarge: boolean = false;

    public languages: ILanguage[] = [];

    constructor(
        private readonly localStorageService: LocalStorageService,
        private readonly translateService: TranslateService,
    ) { }

    public ngOnInit(): void {
        this.languages = [
            { image: 'assets/flags/gb.svg', name: 'English', slug: 'en' },
            { image: 'assets/flags/pl.svg', name: 'Polski', slug: 'pl' },
        ];
    }

    public onFlagClick(lang: ILanguage): void {
        this.localStorageService.setValue(LocalStorageKeys.language, lang.slug);
        this.localStorageService.setValue(LocalStorageKeys.isLanguageSelected, "1")
        this.translateService.use(lang.slug)
        window.location.reload();
    }
}
