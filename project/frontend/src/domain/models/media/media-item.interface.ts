import { IMediaItemImage } from "./media-item-image.interface";
import { IMediaItemLanguage } from "./media-item-language.interface";
import { IMediaItemLink } from "./media-item-link.interface";
import { IMediaItemTitle } from "./media-item-title.interface";
import { IMediaItemAttribute } from "./media-item-attribute.interface";

export interface IMediaItem {
    id: number;
    image: string;
    title: string;
    chapterTitle: string;
    description: string;
    instructions: string;
    path: string;
    isGrouping: boolean;
    group: string;
    groupId: number;
    attributes: IMediaItemAttribute[];
    images: IMediaItemImage[];
    links: IMediaItemLink[];
    titles: IMediaItemTitle[];
    languages: IMediaItemLanguage[];
}
