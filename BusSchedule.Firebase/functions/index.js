const functions = require("firebase-functions");
const admin = require('firebase-admin');

const defaultApp = admin.initializeApp(functions.config().firebase, "default");
const newsApp = admin.initializeApp({databaseURL:"https://busschedule-news.europe-west1.firebasedatabase.app/"}, "news");


exports.latestScheduleOld = functions.https.onRequest( async (request, response) => {
    const result = await admin.database(defaultApp).ref('/').once('value');
    let data = result.val();
    console.log(request.query);
    
    let dateToCheckFor = new Date();
    let nearestDate = data[0];
    let cachedDiff = Infinity;
    data.forEach(value => {
        let date = new Date(value.start_date);
        console.log(date);
        let diff = dateToCheckFor.getTime() - date.getTime();
        if (diff > 0 && diff < cachedDiff) {
            cachedDiff = diff;
            nearestDate = value;
        }
    });

    console.log(dateToCheckFor);
    console.log(nearestDate);

    var filename = nearestDate.filename;
    var start_date = nearestDate.start_date;

    var obj = new Object();
    obj.filename = filename;
    obj.startdate = start_date;
    response.send(JSON.stringify(obj));
});

exports.latestSchedule = functions.https.onRequest( async (request, response) => {
    const result = await admin.database(defaultApp).ref('/').once('value');
    let data = result.val();
    var appVersion = 200;
    if(request.query.app_version != null)
    {
        appVersion = request.query.app_version;
        console.log(request.query.app_version);
    }
    
    let dateToCheckFor = new Date();
    let nearestDate = data[0];
    let cachedDiff = Infinity;
    data.forEach(value => {
        let date = new Date(value.start_date);
        let diff = dateToCheckFor.getTime() - date.getTime();
        let minVersion = value.min_version;
        if (diff > 0 && diff < cachedDiff && appVersion >= minVersion) {
            cachedDiff = diff;
            nearestDate = value;
        }
    });

    var filename = nearestDate.filename;
    var start_date = nearestDate.start_date;

    var obj = new Object();
    obj.filename = filename;
    obj.startdate = start_date;
    response.send(JSON.stringify(obj));
});

exports.getNews = functions.https.onRequest( async (request, response) => {
    const result = await admin.database(newsApp).ref('/').once('value');
    let data = result.val();
    let resultArray = new Array();
    data.forEach(value => {
        //console.log(value);
        var obj = new Object();
        obj.show = value.show;
        obj.title = value.title;
        obj.message = value.message;
        resultArray.push(obj);
    });
    response.send(JSON.stringify(resultArray));
});
