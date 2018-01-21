var LoadingIndicator = React.createClass({
    mixins: [FluxChildMixin, StoreWatchMixin("CommonStateStore")],

    getStateFromFlux: function () {
        return {
            isLoading: this.getFlux().store("CommonStateStore").isLoading()
        };
    },

    render: function () {
        let className = "spinner";

        if (this.state.isLoading) {
            className += " spinner-spin";
        }
        
        return (
            <div className={className}></div>
        );
    }
    
});