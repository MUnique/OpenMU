var Layout = React.createClass({
    mixins: [FluxMixin],
    getInitialState: function () {
        return {
            currentContent: Contents.SERVER_LIST
        };
    },

    render: function () {
        return (
            <div>
                <header>
                    <HeaderMenu setContent={this.setContent} currentContent={this.state.currentContent} />
                </header>
                
                <aside>
                    {
                        this.state.currentContent === Contents.SERVER_LIST ? <ServerList flux={this.getFlux()} /> :
                            this.state.currentContent === Contents.ACCOUNT_LIST ? <AccountList flux={this.getFlux()} /> :
                                this.state.currentContent === Contents.LOG_VIEW ? <LogTable flux={this.getFlux()} /> :
                                null
                    }
                </aside>
                
                <footer />
            </div>);
    },

    setContent(newContent) {
        this.setState({ currentContent : newContent });
    }
});