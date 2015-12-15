//not used
Meteor.startup(function() {
  var masterJson = JSON.parse(Assets.getText("trpg.json"));
});

Meteor.methods({
  'getHelloWorld': function() {
    console.log("Hello world from the the meteor server!");
    return "Hello world from meteor server!";
  },
  'getSpellsData': function() { //hardcoded to get the first actor for now
    var masterJson = JSON.parse(Assets.getText("trpg.json"))
    return masterJson.actors[0].actions;
  }

});
