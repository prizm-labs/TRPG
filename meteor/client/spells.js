var app = angular.module('combatApp', [
  'angular-meteor',
  'ui.router',
  'ionic',
  'ngCordova.plugins.datePicker']);


function onReady() {
  angular.bootstrap(document, ['combatApp'], {strictDi: true});
}

if (Meteor.isCordova) {
  angular.element(document).on("deviceready", onReady);
}
else {
  angular.element(document).ready(onReady);
}



// subscribe to the two collections we use
Meteor.subscribe('Projects');
Meteor.subscribe('Tasks');


app.config(function($stateProvider, $urlRouterProvider) {

  $stateProvider
    .state('tabs', {
      url: "/tab",
      abstract: true,
      templateUrl: "templates/tabs.ng.html"
    })
    .state('tabs.home', {
      url: "/home",
      views: {
        'home-tab': {
          templateUrl: "templates/home.ng.html",
          controller: 'SpellTabCtrl'
        }
      }
    })
    .state('tabs.facts', {
      url: "/facts",
      views: {
        'home-tab': {
          templateUrl: "templates/facts.ng.html"
        }
      }
    })
    .state('tabs.facts2', {
      url: "/facts2",
      views: {
        'home-tab': {
          templateUrl: "templates/facts2.ng.html"
        }
      }
    })
    .state('tabs.about', {
      url: "/about",
      views: {
        'about-tab': {
          templateUrl: "templates/about.ng.html"
        }
      }
    })
    .state('tabs.navstack', {
      url: "/navstack",
      views: {
        'about-tab': {
          templateUrl: "templates/nav-stack.ng.html"
        }
      }
    })
    .state('tabs.contact', {
      url: "/contact",
      views: {
        'contact-tab': {
          templateUrl: "templates/contact.ng.html"
        }
      }
    });
   $urlRouterProvider.otherwise("/tab/home");

});

app.controller('SpellTabCtrl', function($scope, $ionicModal) {
  console.log('SpellTabCtrl');


  $scope.spells = [{'name': "Hello Kitty", 'damage': 3, 'imgSrc':"img/rogue_portrait.jpg", 'targetingPattern':'single'},
              {'name': "Dunkey Roll", 'damage': 'millions', 'imgSrc':'img/bullsh.jpg', 'targetingPattern':'angle'},
              {'name': "I'm my own Grandpa", 'damage': 24, 'imgSrc':'img/rogue_portrait.jpg', 'targetingPattern':'mapPoint'},
              {'name': "Benson Turnover", 'damage': 38, 'imgSrc':'img/rogue_portrait.jpg', 'targetingPattern':'line'},
              {'name': "Home Alone 6", 'damage': 40, 'imgSrc':'img/rogue_portrait.jpg', 'targetingPattern':'multi'}
            ];

  $scope.availableTargets = [
    {'name':'Jason Statham', 'hp':'trillions', 'imgSrc':'img/jason.jpg', 'selected':false},
    {'name':'Bill', 'hp':10, 'imgSrc':'img/batman.jpg', 'selected':false},
    {'name':'Sparky the WonderDog', 'hp':79, 'imgSrc':'img/sparky.jpg', 'selected':false},
  ];

  $scope.selected = {
    'name':'bill'
  };

  $scope.toggleMultiSelected = function(targetName) {
    for (var i = 0; i < $scope.availableTargets.length; i++) {
      if ($scope.availableTargets[i].name == targetName)
      {
        $scope.availableTargets[i].selected = !$scope.availableTargets[i].selected;
      }
    }
    console.log("have toggled: " + targetName);
  };

  $ionicModal.fromTemplateUrl('templates/attackPatterns/mapPoint.ng.html', {
      scope: $scope,
      animation: 'slide-in-up'
    }).then(function(modal) {
      $scope.modal = modal;
    });

  $scope.openModal = function(targetingPattern){

    $ionicModal.fromTemplateUrl('templates/attackPatterns/' + targetingPattern + '.ng.html', {
        scope: $scope,
        animation: 'slide-in-up'
      }).then(function(modal) {
        $scope.modal = modal;
        $scope.modal.show();
      });

      if (targetingPattern == "angle" && !this.angleHandlerSet) {
        this.angleHandlerSet = true;
        $scope.$on('modal.shown', function(event, modal) {
          console.log("angle is the modal!!");
            var width = 1000;
            var height = 600;
            var canvas = ctx = false;
            var frameRate = 1/40; // Seconds
            var frameDelay = frameRate * 1000; // ms
            var loopTimer = false;

            var center = {x: width / 2, y: height / 2};

            var mouse = {x: 0, y: 0, isDown: false};

            function getMousePosition(e) {
                mouse.x = e.pageX - canvas.offsetLeft;
                mouse.y = e.pageY - canvas.offsetTop;
            }
            var mouseDown = function(e) {
                if (e.which == 1) {
                    getMousePosition(e);
                    mouse.isDown = true;
                }
            }
            var mouseUp = function(e) {
                if (e.which == 1) {
                    mouse.isDown = false;
                    $scope.finalizeAngleAttack = true;
                    console.log("moused up: " + $scope.finalizeAngleAttack);
                }
            }

            var setup = function() {
                canvas = document.getElementById("canvas");
                ctx = canvas.getContext("2d");

                //set up handlers
                canvas.onmousemove = getMousePosition;
                canvas.onmousedown = mouseDown;
                canvas.onmouseup = mouseUp;

                ctx.fillStyle = 'red';
                ctx.lineWidth=10;
                ctx.strokeStyle = '#000000';
                loopTimer = setInterval(loop, frameDelay);
            }

            var loop = function() {
              if (mouse.isDown) ctx.clearRect(0,0,width,height);
              //draw red ball
              ctx.save();
              ctx.translate(center.x, center.y);
              ctx.beginPath();
              ctx.arc(0, 0, 20, 0, Math.PI*2, true);
              ctx.fill();
              ctx.closePath();
              ctx.restore();

              if (mouse.isDown) {
                ctx.beginPath();
                ctx.moveTo(center.x, center.y);
                ctx.lineTo(mouse.x - 331, mouse.y - 200);
                ctx.stroke();
                ctx.closePath();
              }
            }

            setup();
        });
      }

    console.log(targetingPattern);
  }
  $scope.closeModal = function(){
    $scope.modal.hide();
  }
  $scope.LaunchAttack = function() {
    //put the meteor call here
    console.log("launching attack!");

    for (var i = 0; i < $scope.availableTargets.length; i++) {  //clear the selected targets
      console.log($scope.availableTargets[i].name + " is: " + $scope.availableTargets[i].selected);
      $scope.availableTargets[i].selected = false;
    }
    $scope.closeModal();
  }
});
