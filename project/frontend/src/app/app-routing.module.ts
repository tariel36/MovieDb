import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LibraryItemDetailsComponent } from '../domain/library/components/library-item-details/library-item-details.component';
import { LibraryComponent } from '../domain/library/components/library/library.component';
import { OptionsContainerComponent } from '../domain/options/components/options-container/options-container.component';

const routes: Routes = [
    { path: 'options', component: OptionsContainerComponent },
    { path: 'details/:id', component: LibraryItemDetailsComponent },
    { path: '**', component: LibraryComponent },
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }
