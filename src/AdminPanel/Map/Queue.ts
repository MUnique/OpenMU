export class Queue<T> {
    private _oldestIndex: number;
    private _newestIndex: number;
    private _storage: T[];

    constructor() {
        this._oldestIndex = 1;
        this._newestIndex = 1;
        this._storage = [];
    }

    public size(): number {
        return this._newestIndex - this._oldestIndex;
    }

    public enqueue(data: T): void {
        this._storage[this._newestIndex] = data;
        this._newestIndex++;
    }

    public dequeue(): T {
        const oldestIndex = this._oldestIndex;
        const newestIndex = this._newestIndex;

        if (oldestIndex !== newestIndex) {
            const deletedData = this._storage[oldestIndex];
            delete this._storage[oldestIndex];
            this._oldestIndex++;

            return deletedData;
        }

        return null;
    }

    public peek(): T {
        if (this._oldestIndex !== this._newestIndex) {
            return this._storage[this._oldestIndex];
        }

        return null;
    }
}
