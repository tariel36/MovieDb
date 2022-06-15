import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LanguageSelectionComponent } from './components/language-selection/language-selection.component';
import { TranslateModule } from '@ngx-translate/core';
import { MaterialModule } from '../../material/material.module';



@NgModule({
  declarations: [
    LanguageSelectionComponent,
  ],
  imports: [
    CommonModule,
    TranslateModule,
    MaterialModule
  ],
  exports: [LanguageSelectionComponent]
})
export class LanguagesModule { }
