import { Injectable } from "@angular/core";
import { TranslateService } from "@ngx-translate/core";
import { Maybe } from "../../models/utility/maybe.type";
import { LocalStorageKeys } from "../../utility/local-storage-keys.enum";
import { LocalStorageService } from "../../utility/local-storage.service";
import { ILanguage } from "../models/language.interface";

@Injectable({ providedIn: 'root' })
export class LanguageSelectionService {
    public readonly languages: ILanguage[] = [];

    constructor(
        private readonly localStorageService: LocalStorageService,
        private readonly translateService: TranslateService,
    ) {
        this.languages = [
            { image: 'assets/flags/gb.svg', name: 'English', slug: 'en' },
            { image: 'assets/flags/pl.svg', name: 'Polski', slug: 'pl' },
        ];
    }

    public getLanguage(): Maybe<ILanguage> {
        const slug = this.localStorageService.getValue(LocalStorageKeys.language);
        return this.languages.find(x => x.slug === slug);
    }

    public setLanguage(lang: ILanguage): void {
        this.localStorageService.setValue(LocalStorageKeys.language, lang.slug);
        this.localStorageService.setValue(LocalStorageKeys.isLanguageSelected, "1")
        this.translateService.use(lang.slug)
    }
}
