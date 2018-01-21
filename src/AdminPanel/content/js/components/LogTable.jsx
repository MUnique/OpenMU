var LogTable = React.createClass({
  mixins: [FluxMixin, StoreWatchMixin("LogStore")],
  
  getStateFromFlux: function() {
    var logStore = this.getFlux().store("LogStore");
    return {
      entries: logStore.getAll(),
      loggers: logStore.getLoggers(),
      autoRefresh: logStore.getAutoRefresh()
    };
  },

  componentDidMount: function() {
      this.getFlux().actions.logSubscribe();
  },

  componentWillUnmount: function() {
      this.getFlux().actions.logUnsubscribe();
  },

  render: function() {
    var entryList = this.state.entries.map(function (entry) {
      return (
        <LogEntry entry={entry} key={entry.timeStamp}/>
      );
    });
    
    return (
        <div className="log">
          <LogFilter loggers={this.state.loggers} autoRefresh={this.state.autoRefresh} key="logFilter"/>
          <table className="log">
            <thead>
              <tr>
                <th className="col-xs-1">Timestamp</th>
                <th className="col-xs-1">Logger</th>
                <th className="col-xs-4">Message</th>
              </tr>
            </thead>
            <tbody>
                {entryList}
            </tbody>
          </table>
        </div>
        );
  }
});


var LogFilter = React.createClass({
  mixins: [FluxChildMixin],
  
  filterByServer: function() {
    var server = $('#server')[0].value;
    this.getFlux().actions.filterLogByServer(server);
  },
  
  filterByCharacter: function() {
    var character = $('#character')[0].value;
    this.getFlux().actions.filterLogByCharacter(character);
  },
  
  filterByLogger: function() {
    var logger = $('#logger')[0].value;
    this.getFlux().actions.filterLogByLogger(logger);
  },
  
  activeChanged: function() {
    this.getFlux().actions.setLogAutoRefresh($('#myonoffswitch')[0].checked);
  },
  
  render: function() {
    var loggerList = this.props.loggers.map(function (logger, index) {
            return (<option value={logger} key={index} />);
        });
    var checkBox;
    if (this.props.autoRefresh === true) {
      checkBox = (<input type="checkbox" id="myonoffswitch" onClick={this.activeChanged} value={this.props.autoRefresh} checked />);
    } else {
      checkBox = (<input type="checkbox" id="myonoffswitch" onClick={this.activeChanged} value={this.props.autoRefresh} />);
    }
    
    return (
        <div>Filters:
            <datalist id="loggers">
              {loggerList}
            </datalist>
            <input id="server" type="text" placeholder="Server" onChange={this.filterByServer} />
            <input id="character" type="text" placeholder="Character"  onChange={this.filterByCharacter}/>
            <input id="logger" type="text" list="loggers" placeholder="Logger" width="200" onChange={this.filterByLogger} />
            <div> {checkBox} Auto Refresh</div>
        </div>
    );
  }
});