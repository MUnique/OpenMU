import React from "react";

import { ApplicationState } from "../stores/index";

import AccountList from "./AccountList";
import {HeaderMenu}  from "./HeaderMenu";
import ServerList from "./ServerList";
import LogTable from "./LogTable";
import SystemMonitor from "./SystemMonitor";
import PlugInList from "./PlugInList";
import Modal from "./Modal";

interface LayoutState {
    currentContent: Contents;
}


export enum Contents  {
    SERVER_LIST = 0,
    ACCOUNT_LIST = 1,
    LOG_VIEW = 2,
    SYSTEM = 3,
    PLUGINS = 4,
};



export class Layout extends React.Component<{}, LayoutState>{
    constructor(props: {}) {
        super(props);
        this.state = { currentContent: Contents.SERVER_LIST}
    }

    public render() {
        return (
            <div>
                <header>
                    <HeaderMenu setContent={(newContent: number) => this.setContent(newContent)} currentContent={this.state.currentContent}/>
                </header>

                <aside>
                    {
                        // TODO: replace by redux-first-router
                        this.state.currentContent === Contents.SERVER_LIST
                            ? <ServerList/>
                            : this.state.currentContent === Contents.ACCOUNT_LIST
                            ? <AccountList/>
                            : this.state.currentContent === Contents.LOG_VIEW
                            ? <LogTable/>
                            : this.state.currentContent === Contents.SYSTEM
                            ? <SystemMonitor />
                            : this.state.currentContent === Contents.PLUGINS
                            ? <PlugInList />
                            : null
                    }

                    <Modal />
                </aside>
                <footer>
                </footer>
            </div>
        );
    }

    private setContent(newContent: Contents) {
        this.setState({ currentContent: newContent });
    }
}
