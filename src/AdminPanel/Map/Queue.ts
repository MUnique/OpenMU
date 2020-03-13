export class Queue<T> {
    private _oldestIndex: number;
    private _newestIndex: number;
    private _storage: T[];

    constructor() {
        this._oldestIndex = 1;
        this._newestIndex = 1;
        this._storage = [];
    }

    size() {
        return this._newestIndex - this._oldestIndex;
    }

    enqueue(data: T) {
        this._storage[this._newestIndex] = data;
        this._newestIndex++;
    }

    dequeue() {
        let oldestIndex = this._oldestIndex,
            newestIndex = this._newestIndex;

        if (oldestIndex !== newestIndex) {
            let deletedData = this._storage[oldestIndex];
            delete this._storage[oldestIndex];
            this._oldestIndex++;

            return deletedData;
        }

        return null;
    }

    peek() {
        if (this._oldestIndex !== this._newestIndex) {
            return this._storage[this._oldestIndex];
        }

        return null;
    }
}