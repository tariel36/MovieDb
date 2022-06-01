import { Injectable } from "@angular/core";
import { Maybe } from "../models/utility/maybe.type";

@Injectable({ providedIn: 'root' })
export class LocalStorageService {
    public getValue(key: string, defaultValue?: string): Maybe<string> {
        return localStorage.getItem(key) ?? defaultValue;
    }
    
    public setValue(key: string, value: string): void {
        localStorage.setItem(key, value);
    }
}
