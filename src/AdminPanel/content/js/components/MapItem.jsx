var MapItem = React.createClass({
  mixins: [FluxChildMixin],

  handleClick: function(event) {
      var liveMapUrl = this.getLiveMapUrl();
      window.open(liveMapUrl, "_blank");
  },

  getLiveMapUrl: function() {
      return "/admin/livemap?serverId=" + this.props.map.serverId + "&mapId=" + this.props.map.id;
  },
  
  render: function () {
      var divId = "livemap_" + this.props.map.serverId + "_" + this.props.map.id;
      var divIdSelector = "#" + divId;
      var liveMapUrl = this.getLiveMapUrl();
      var iFrameId = divId + "_frame";
      $(divIdSelector).on('shown.bs.modal', function () {
          $('#' + iFrameId).prop("src", function () {
              // Set their src attribute to the value of data-src
              return $(this).data("src");
          });
      });

      $(divIdSelector).on('hide.bs.modal', function () {
          $('#' + iFrameId).prop("src", function () {
              return "";
          });
      });

    return (
        <tr>
            <td>{this.props.map.name}</td>
            <td>{this.props.map.playerCount}</td>
            <td>
                <button type="button" className="btn btn-default btn-xs glyphicon glyphicon-modal-window" data-toggle="modal" data-target={divIdSelector}></button>
                <button type="button" className="btn btn-default btn-xs glyphicon glyphicon-new-window" onClick={this.handleClick}></button>
                <div id={divId} className="modal fade" role="dialog">
                    <div className="modal-dialog modal-dialog-fullpage">
                        <div className="modal-content modal-content-fullpage">
                            <div className="modal-header">
                            <button type="button" className="close" data-dismiss="modal">&times;</button>
                            <h4 className="modal-title">{this.props.map.name} Live Map</h4>
                            </div>
                            <div className="modal-body modal-body-fullpage">
                                <iframe id={iFrameId} data-src={liveMapUrl} width="100%" height="100%" frameborder="0"/>
                            </div>
                        </div>
                    </div>
                </div>
            </td>
        </tr>
    );
  }
});
