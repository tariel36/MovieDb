import { Injectable } from "@angular/core";
import { MediaItemsService } from "../media-items/services/media-items.service";
import { IMediaItemImage } from "../models/media/media-item-image.interface";

@Injectable({ providedIn: 'root' })
export class ImageSourceResolverService {
    constructor(private readonly mediaItemsService: MediaItemsService) {

    }

    public resolve(item: IMediaItemImage): string {
        if (!item.image) {
            return "/assets/placeholders/no-cover.png";
        }

        return item.image.toLowerCase().startsWith("http")
            ? item.image
            : this.mediaItemsService.getImageUrl(item.id)
    }
}