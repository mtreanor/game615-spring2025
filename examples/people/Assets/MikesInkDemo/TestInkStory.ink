// This is a basic example
== IntroductoryScene ===
Once upon a time, in a magic forest
Lived a fox that loved to eat Acai bowls

What should he do?
->ChooseAction


== ChooseAction ===
* Walk into the trees
    The trees were very inviting. The fox had a nice day.
    -> DONE
* Peek inside the glowing cave?
    The fox saw a bunch of rocks that emitted light
    ** Further investigate
        Right there between two glowing rocks was a tasty acai bowl!
        *** Eat
            Yum!
            -> DONE
        *** Leave
            ->ChooseAction
    ** Go back
        ->ChooseAction
    -> DONE 





// This knot demonstrates how you can create
// parameterized / generalized patterns of 
// conditional conversation between characters
// This relies on the following variables being set before being called
VAR player = "N/A"
VAR player_charisma = -1
VAR responder = "N/A"
VAR responder_affinityTowardPlayer = -1
=== TalkToCharacter ===

Hello, {responder}.

* Did you know...
    Did you know that you can see the moon in sky during the day sometimes!?!
    -> DONE
* {player_charisma > 5}[Make suave comment...]
    Careful, keep looking at me like that and I’ll start thinking you mean it.
    {responder_affinityTowardPlayer > 4: 
        {responder} keeps looking at {player} like that.
        -else: 
            {responder} rolls their eyes.
    }
    -> DONE




