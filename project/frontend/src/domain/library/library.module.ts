import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CollectionComponent } from './components/collection/collection.component';
import { LibraryComponent } from './components/library/library.component';
import { LibraryItemComponent } from './components/library-item/library-item.component';
import { MaterialModule } from '../../material/material.module';
import { LibraryItemDetailsComponent } from './components/library-item-details/library-item-details.component';
import { TranslateModule } from '@ngx-translate/core';
import { MediaItemsModule } from '../media-items/media-items.module';

@NgModule({
    declarations: [
      CollectionComponent,
      LibraryComponent,
      LibraryItemComponent,
      LibraryItemDetailsComponent,
    ],
    imports: [
      CommonModule,
      MaterialModule,
      TranslateModule,
      MediaItemsModule
    ]
})
export class LibraryModule { }
