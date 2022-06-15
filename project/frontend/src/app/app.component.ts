import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { LightboxConfig } from 'ngx-lightbox';
import { LocalStorageKeys } from '../domain/utility/local-storage-keys.enum';
import { LocalStorageService } from '../domain/utility/local-storage.service';
import { MatDialog } from '@angular/material/dialog';
import { MediaItemsService } from '../domain/media-items/services/media-items.service';
import { OptionsContainerDialogComponent } from '../domain/options/components/options-container-dialog/options-container-dialog.component';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
    public title = 'MovieDb';

    constructor(
        private readonly mediaItemsService: MediaItemsService,
        private readonly translateService: TranslateService,
        private readonly localStorageService: LocalStorageService,
        private readonly router: Router,
        private readonly dialog: MatDialog,
        private readonly lightboxConfig: LightboxConfig
    ) {
        this.lightboxConfig.fadeDuration = 0.3;
        this.lightboxConfig.centerVertically = true;

        this.translateService.use(this.localStorageService.getValue(LocalStorageKeys.language) ?? 'en');

        this.translateService.get('app-title')
            .toPromise()
            .then(x => this.title = x);
    }
    
    public ngOnInit(): void {
        this.openSettingsDialog(
            OptionsContainerDialogComponent,
            [
                LocalStorageKeys.isNotificationSelected,
                LocalStorageKeys.isMediaItemTypesSelected,
                LocalStorageKeys.isLanguageSelected
            ]
        );
    }

    public onTitleClick(): void {
        this.router.routeReuseStrategy.shouldReuseRoute = () => false;
        this.router.navigate(['/']);
    }

    public onOptionsClick(): void {
        this.router.navigateByUrl('/options')
    }

    public onRandomClick(): void {
        const types = JSON.parse(this.localStorageService.getValue(LocalStorageKeys.selectedMediaItemTypes) ?? '[]') as number[];
        this.mediaItemsService.getRandom(types)
            .then(id => {
                this.router.routeReuseStrategy.shouldReuseRoute = () => false;
                this.router.navigateByUrl(`/details/${id}`);
            });
    }

    private openSettingsDialog(component: any, keys: string[]): void {
        if (keys.some(x => Number.parseInt(this.localStorageService.getValue(x) ?? '0') == 0)) {
            this.dialog.open(component);
        }
    }
}
