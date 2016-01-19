app.controller('DebugTabCtrl', function($scope, $ionicModal, socket) {

  console.log('DebugTabCtrl');

  $scope.characters = {
    "wizard":{
      "abilities":["coneofcold","fireball","magicmissile"]
    },
    "warrior":{
      "abilities":["axe-melee","axe-ranged"]
    },
    "cleric":{
      "abilities":["heal","hammer"]
    },
    "ranger":{
      "abilities":["arrow","dual-swords"]
    }
  }

  $scope.chooseCharacter = chooseCharacter;

  $scope.sendCharacterData = sendCharacterData;
  $scope.sendActiveTurn = sendActiveTurn;
  $scope.sendSingleTargetSelection = sendSingleTargetSelection;

  function chooseCharacter(id) {
    $scope.selectedCharacter = $scope.characters[id];
    $scope.selectedCharacterKey = id;
    sendCharacterData(id);
  }

  function startTurn() {
    var topic = "start-turn";
    sendToTabletop(topic,selectedCharacterKey);
  }

  function sendCharacterData(id){
    sendToTabletop($scope.characters[id]);
  }

  function sendActiveTurn(id) {
    var topic = "active-turn";
    sendToTabletop(topic,$scope.characters[id]);
  }

  function sendAbilitySelection(id) {
    var topic = "ability-select";
    $scope.selectedAbility = id;
    sendToTabletop(topic,id);
    // receive valid targets
  }

  function showAbilityTargets(data) {
    $scope.targetType = data.type;

     switch (data.type) {

       case "single":
        $scope.targets = data.targets;
        break;
     }

  }

  function sendSingleTargetSelection(id) {
    var topic = "target-select";
    sendToTabletop(topic,id);
    // receive confirmation of selection
  }

  function sendTargetConfirmation() {
    var topic = "target-confirm";
    sendToTabletop(topic,$scope.selectedCharacterKey);
  }

  function sendAbilityCancellation() {
    var topic = "ability-cancel";
    sendToTabletop(topic,$scope.selectedAbility);

    $scope.selectedAbility = null;
  }


});
