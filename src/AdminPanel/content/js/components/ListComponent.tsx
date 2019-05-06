import React from "react";

export interface IListProps {
    page: number;
    pageSize: number;
    hasMoreEntries: boolean;
}

export abstract class ListComponent<TProps extends IListProps, TState> extends React.Component<TProps, TState> {
    protected getToolbar() : JSX.Element {
        return (<div className="btn-toolbar" role="toolbar" aria-label="Toolbar with button groups">
                    <div className="btn-group" role="group" aria-label="Navigation group">
                        <button type="button" className={this.getPreviousButtonClass()} onClick={() => this.fetchPreviousPage()}><span className="glyphicon glyphicon-chevron-left" aria-hidden="true"/></button>
                        <button type="button" className="btn btn-xs disabled">Page {this.props.page}</button>
                        <button type="button" className={this.getNextButtonClass()} onClick={() => this.fetchNextPage()}><span className="glyphicon glyphicon-chevron-right" aria-hidden="true"/></button>
                    </div>
                </div>);
    }

    abstract fetchNextPage() : void;

    abstract fetchPreviousPage() : void;

    private getPreviousButtonClass(): string {
        const buttonClass = "btn btn-xs ";
        if (this.props.page <= 1) {
            return buttonClass + 'disabled';
        }

        return buttonClass;
    }

    private getNextButtonClass(): string {
        const buttonClass = "btn btn-xs ";
        if (!this.props.hasMoreEntries) {
            return buttonClass + 'disabled';
        }

        return buttonClass;
    }
}