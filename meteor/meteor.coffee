if Meteor.isClient
  Session.setDefault 'counter', 0

  Template.hello.helpers
    counter: -> Session.get 'counter'

  Template.hello.events
    'click button': ->
      Session.set 'counter', Session.get('counter') + 1
      Meteor.call "button" 

if Meteor.isServer
  console.log 'This code runs on server'
