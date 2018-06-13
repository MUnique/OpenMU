import Redux from "redux";
import { Constants, SystemUpdateAction } from "./actions";

export type SystemState = {
    readonly snapshots: SystemStateSnapshot[]
};

export interface SystemStateSnapshot {
    readonly timestamp: any;
    readonly cpuPercentTotal: number;
    readonly cpuPercentInstance: number;
    readonly bytesSent: number;
    readonly bytesReceived: number;
}

export const systemStateReducer: Redux.Reducer<SystemState> =
(state: SystemState = { snapshots: [] },
    action: Redux.Action) => {
    switch ((action as Redux.Action).type) {
    case Constants.SYSTEM_UPDATE:
    {
        let updateAction = action as SystemUpdateAction;
        let newSnapshots = state.snapshots.slice(Math.max(0, state.snapshots.length - 29, 29));
        newSnapshots.push({
            timestamp: new Date(),
            cpuPercentTotal: updateAction.cpuPercentTotal,
            cpuPercentInstance: updateAction.cpuPercentInstance,
            bytesReceived: updateAction.bytesReceived,
            bytesSent: updateAction.bytesSent
                });
        return { snapshots: newSnapshots };
    }

    }

    return state;
};