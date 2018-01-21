var HeaderMenu = React.createClass({
    render: function () {
        return (
            <nav className="navbar navbar-inverse navbar-static-top" role="navigation">
                <div className="container-fluid">
                    <div className="navbar-header">
                        <LoadingIndicator />
                    </div>
                    <ul className="nav navbar-nav">
                        <li className={this.props.currentContent === Contents.SERVER_LIST ? "active" : ""}>
                            <a onClick={() => this.props.setContent(Contents.SERVER_LIST)}>Server List</a>
                        </li>
                        <li className={this.props.currentContent === Contents.ACCOUNT_LIST ? "active" : ""}>
                            <a onClick={() => this.props.setContent(Contents.ACCOUNT_LIST)}>Accounts</a>
                        </li>
                        <li className={this.props.currentContent === Contents.LOG_VIEW ? "active" : ""}>
                            <a onClick={() => this.props.setContent(Contents.LOG_VIEW)}>Log</a>
                        </li>
                    </ul>
                </div>
            </nav>
        );
    }
});