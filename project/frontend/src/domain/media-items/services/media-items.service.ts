import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "../../../environments/environment";
import { ICachedImage } from "../../models/media/cached-image.interface";
import { IGroupMediaItem } from "../../models/media/group-media-item.interface";
import { IMediaItem } from "../../models/media/media-item.interface";
import { Maybe } from "../../models/utility/maybe.type";
import { LocalStorageKeys } from "../../utility/local-storage-keys.enum";
import { LocalStorageService } from "../../utility/local-storage.service";

@Injectable({ providedIn: 'root' })
export class MediaItemsService {
    constructor(
        private readonly httpClient: HttpClient,
        private readonly localStorageService: LocalStorageService
    ) {

    }

    public async setNotificationEmail(email: string, mediaItemTypes?: number[] | string[]): Promise<void> {
        await this.httpClient.post<IMediaItem[]>(
            `${environment.baseApiUrl}/DataProvider/options/NotificationEmail`,
            email,
            {
                headers: {
                    MediaItemTypes: (mediaItemTypes ?? []).map(x => x.toString()).join(','),
                    Language: this.localStorageService.getValue(LocalStorageKeys.language, 'en') as string
                }
            }
        ).toPromise();
    }

    public async getRandom(mediaItemTypes?: number[] | string[]): Promise<Maybe<number>> {
        const item = await this.httpClient.get<number>(
            `${environment.baseApiUrl}/DataProvider/random`,
            {
                headers: {
                    MediaItemTypes: (mediaItemTypes ?? []).map(x => x.toString()).join(','),
                    Language: this.localStorageService.getValue(LocalStorageKeys.language, 'en') as string
                }
            }
        ).toPromise();
    
        return item ?? -1;
    }

    public async searchMediaItems(searchText: string, mediaItemTypes?: number[] | string[]): Promise<IMediaItem[]> {
        const items = await this.httpClient.post<IMediaItem[]>(
            `${environment.baseApiUrl}/DataProvider/search`,
            searchText,
            {
                headers: {
                    MediaItemTypes: (mediaItemTypes ?? []).map(x => x.toString()).join(','),
                    Language: this.localStorageService.getValue(LocalStorageKeys.language, 'en') as string
                }
            }
        ).toPromise();
    
        return items ?? [];
    }

    public async getAllMediaItems(mediaItemTypes?: number[] | string[]): Promise<IMediaItem[]> {
        const items = await this.httpClient.get<IMediaItem[]>(
            `${environment.baseApiUrl}/DataProvider`,
            {
                headers: {
                    MediaItemTypes: (mediaItemTypes ?? []).map(x => x.toString()).join(','),
                    Language: this.localStorageService.getValue(LocalStorageKeys.language, 'en') as string
                }
            }
        ).toPromise();
    
        return items ?? [];
    }

    public async getMediaItem(id: Maybe<string> | Maybe<number>): Promise<Maybe<IMediaItem>> {
        return await this.httpClient
            .get<IMediaItem>(
                `${environment.baseApiUrl}/DataProvider/${id}`,
                { headers: { Language: this.localStorageService.getValue(LocalStorageKeys.language, 'en') as string } }
            ).toPromise();
    }

    public async getGroup(id: Maybe<string> | Maybe<number>): Promise<Maybe<IGroupMediaItem>> {
        return await this.httpClient
            .get<IGroupMediaItem>(
                `${environment.baseApiUrl}/DataProvider/group/${id}`,
                { headers: { Language: this.localStorageService.getValue(LocalStorageKeys.language, 'en') as string } }
            ).toPromise();
    }

    public async getDetails(id: Maybe<string> | Maybe<number>): Promise<Maybe<IGroupMediaItem>> {
        return await this.httpClient
            .get<IGroupMediaItem>(
                `${environment.baseApiUrl}/DataProvider/details/${id}`,
                { headers: { Language: this.localStorageService.getValue(LocalStorageKeys.language, 'en') as string } }
            ).toPromise();
    }

    public getImageUrl(id: Maybe<string> | Maybe<number>): string {
        return `${environment.baseApiUrl}/DataProvider/image/${id}`;
    }

    public async getImage(id: Maybe<string> | Maybe<number>): Promise<Maybe<any>> {
        return await this.httpClient.get<any>(`${environment.baseApiUrl}/DataProvider/image/${id}`).toPromise();
    }
}