var LogEntry = React.createClass({
  mixins: [FluxChildMixin],
  
  
  render: function() {
    var dateTime = new Date(Date.parse(this.props.entry.TimeStamp));
    var dateStr = dateTime.toISOString();
    var className = "log " + this.props.entry.Level.Name;
    return (
        
        <tr className={className}>
            <td>{dateStr}</td>
            <td>{this.props.entry.LoggerName}</td>
            <td>{this.props.entry.Message}</td>
        </tr>
    );
  }
});