const functions = require("firebase-functions");
const admin = require('firebase-admin');

const defaultApp = admin.initializeApp(functions.config().firebase, "default");
const newsApp = admin.initializeApp({databaseURL:"https://busschedule-news.europe-west1.firebasedatabase.app/"}, "news");

// // Create and Deploy Your First Cloud Functions
// // https://firebase.google.com/docs/functions/write-firebase-functions
//
exports.helloWorld = functions.https.onRequest((request, response) => {
    functions.logger.info("Hello logs!", { structuredData: true });
    response.send("Hello from Firebase!");
});

exports.latestSchedule = functions.https.onRequest(async (request, response) => {
    const db = admin.database();
    const ref = db.ref('/');
    const data = await ref.once('value');
    var filename = data[0].filename;
    var start_date = date[0].start_date;


    var obj = new Object();
    let currentDate = new Date();
    obj.start_date = currentDate.toLocaleDateString();
    obj.filename = "sqlite.db";
    obj.file = filename;
    obj.date = start_date;
    response.send(JSON.stringify(obj));
});

exports.getNews = functions.https.onRequest( async (request, response) => {
    const result = await admin.database(newsApp).ref('/').once('value');
    let data = result.val();
    let resultArray = new Array();
    data.forEach(value => {
        console.log(value);
        var obj = new Object();
        obj.show = value.show;
        obj.title = value.title;
        obj.message = value.message;
        resultArray.push(obj);
    });
    response.send(JSON.stringify(resultArray));
});
