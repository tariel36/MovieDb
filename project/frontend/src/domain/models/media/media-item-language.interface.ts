import { MediaLanguageType } from "./media-language-type.enum";


export interface IMediaItemLanguage {
    typeName: string;
    language: string;
    type: MediaLanguageType;
}
