import { Component, Input, OnInit } from '@angular/core';
import { ILanguage } from '../../models/language.interface';
import { LanguageSelectionService } from '../../services/language-selection.service';

@Component({
  selector: 'app-language-selection',
  templateUrl: './language-selection.component.html',
  styleUrls: ['./language-selection.component.scss']
})
export class LanguageSelectionComponent implements OnInit {

    @Input() public isLarge: boolean = false;

    public languages: ILanguage[] = [];

    constructor(
        private readonly languageSelectionService: LanguageSelectionService,
    ) { }

    public ngOnInit(): void {
        this.languages = this.languageSelectionService.languages;
    }

    public onFlagClick(lang: ILanguage): void {
        this.languageSelectionService.setLanguage(lang);
        window.location.reload();
    }
}
