const INITIAL_HEAD_INDEX = 0;

export class Queue<T> {
    private storage: T[];
    private headIndex: number;

    constructor() {
        this.storage = [];
        this.headIndex = INITIAL_HEAD_INDEX;
    }

    public size(): number {
        return this.storage.length - this.headIndex;
    }

    public enqueue(data: T): void {
        this.storage.push(data);
    }

    public dequeue(): T {
        if (this.size() === 0) {
            return null;
        }

        const data = this.storage[this.headIndex];
        this.headIndex++;

        this.compactIfRequired();

        return data;
    }

    public peek(): T {
        if (this.size() === 0) {
            return null;
        }

        return this.storage[this.headIndex];
    }

    /*
     * Removes the already dequeued entries from the front of the storage, as soon as
     * they take up half of it. Without that, the storage would grow indefinitely and
     * degrade into a slow dictionary-like array. Because it just happens when half of
     * the entries are dequeued, the costs are amortized constant per dequeued entry.
     */
    private compactIfRequired(): void {
        const compactionThresholdDivisor = 2;
        if (this.headIndex >= this.storage.length / compactionThresholdDivisor) {
            this.storage = this.storage.slice(this.headIndex);
            this.headIndex = INITIAL_HEAD_INDEX;
        }
    }
}
