import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OptionsContainerComponent } from './components/options-container/options-container.component';
import { VisibleMediaItemTypesComponent } from './components/visible-media-item-types/visible-media-item-types.component';
import { VisibleMediaItemTypesDialogComponent } from './components/visible-media-item-types-dialog/visible-media-item-types-dialog.component';
import { MaterialModule } from '../../material/material.module';
import { LanguagesModule } from '../languages/languages.module';
import { TranslateModule } from '@ngx-translate/core';



@NgModule({
  declarations: [
    OptionsContainerComponent,
    VisibleMediaItemTypesComponent,
    VisibleMediaItemTypesDialogComponent
  ],
  imports: [
    CommonModule,
    MaterialModule,
    LanguagesModule,
    TranslateModule
  ],
  exports: [
    OptionsContainerComponent,
    VisibleMediaItemTypesComponent,
    VisibleMediaItemTypesDialogComponent
  ]
})
export class OptionsModule { }
