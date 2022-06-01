import { IMediaItem } from "./media-item.interface";

export interface IGroupMediaItem {
    groupingItem: IMediaItem;
    items: IMediaItem[];
}