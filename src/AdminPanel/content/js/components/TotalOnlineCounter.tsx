import React from "react";
import {Server} from "../stores/servers/types";

export interface TotalOnlineCounterProps {
    servers: Server[];
}

export class TotalOnlineCounter extends React.Component<TotalOnlineCounterProps, any> {

    private summarizePlayerCount() {
        var servers = this.props.servers;
        var count = 0;
        for (var i = 0; i < servers.length; i++) {
            if (servers[i].id < 0x10000) { // we only count the game servers
                count += servers[i].onlinePlayerCount;
            }
        }

        return count;
    }

    public render() {
        return (
            <tr className="info">
                <td colSpan={5}>
                    <span>Total Players: {this.summarizePlayerCount()}</span>
                </td>
            </tr>
        );
    }
}
