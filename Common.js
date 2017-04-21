$.cc = $.extend($.cc || {}, {
    parseJSONDate: function (jsonDate, includeTime) {
        if (jsonDate == null || jsonDate == "") return "";
        if (jsonDate.indexOf("Date") < 0)
        {
            jsonDate = jsonDate.replace(/&quot;/g, "");//??
        }
        else
        {
            jsonDate=parseInt(jsonDate.replace("/Date(","").replace(")/",""),10);
        }
        var utcDate = new Date(jsonDate);
        var localDate = new Date(utcDate.toString());
        var returnValue = localDate.getFullYear() + "/" + (localDate.getMonth() + 1) + "/" + localDate.getDate();
        if (includeTime == true) {
            var minutes = localDate.getMinutes();
            return returnValue + " " + localDate.getHours() + ":" + (minutes < 10 ? "0" + minutes : minutes);
        }
        else
            return returnValue;
    },
})