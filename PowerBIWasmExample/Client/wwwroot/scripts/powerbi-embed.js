export function embedReport(reportContainer, reportId, embedUrl, token) {
    // Embed reprt using the Power BI JavaScrit API.
    var models = window['powerbi-client'].models

    var config = {
        type: 'report',
        id: reportId,
        embedUrl: embedUrl,
        accessToken: token,
        permissions: models.Permissions.All,
        tokenType: models.TokenType.Embed,
        viewMode: models.ViewMode.View,
        settings: {
            navContentPaneEnabled: false,
            panes: {
                filters: { expanded: false, visible: true },
                pageNavagation: {visible: false}
            }
        }
    }

    // Embed the report and display it within the div container
    powerbi.embed(reportContainer, config)

    // Got this from a youtube video. I don't want to include jquery and it seems to work the same with out it.
    //var heightBuffer = 32;
    //var newHeight = $(window).height() - ($("header").height() + heightBuffer)
    //$("#" + containerId).height(newHeight)
    //$(window).resize(() => {
    //    var newHeight = $(window).height() - ($("header").height() + heightBuffer)
    //    $("#" + containerId).height(newHeight)
    //})
}