$(function () {

    $("#explore").addClass("btn-primary");

    var inputBase = $("#input_baseUrl");
    var tagName = inputBase.prop("tagName");

    var defaultVal = inputBase.val();

    if (tagName != "SELECT" && swashbuckleConfig.discoveryPaths.length > 1) {
       
        var select = $('<select id="input_baseUrl" name="baseUrl"></select>');

        select
            //.css('margin', '0')
            //.css('border', '1px solid gray')
            //.css('padding', '3px')
            //.css('font-size', '0.9em')
            .css('width', '400px');
                   

        var rootUrl = swashbuckleConfig.rootUrl;
        $.each(swashbuckleConfig.discoveryPaths, function (index, path) {
            var url = rootUrl + "/" + path;
            console.log("path : " + path);
            console.log("url : " + url);
            var arrs = path.split('/');
            
            var fileName = arrs[arrs.length - 1].replace(".js", "");
            var option = $('<option value=' + url + '>' + fileName + '</option>');
            select.append(option);
        });

        select.val(defaultVal);
        inputBase.replaceWith(select);
    }
});