const functions = require("firebase-functions");

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
