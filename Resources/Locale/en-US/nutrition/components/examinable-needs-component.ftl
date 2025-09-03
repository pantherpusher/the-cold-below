examinable-need-header = [bold][underline][color={$color}]{$needname}[/color][/underline][/bold]

# Hunger
examinable-need-hunger-extrasatisfied  = {CAPITALIZE(SUBJECT($entity))} {CONJUGATE-BASIC($entity, "look", "looks")} stuffed!
examinable-need-hunger-satisfied       = {CAPITALIZE(SUBJECT($entity))} {CONJUGATE-BASIC($entity, "look", "looks")} content.
examinable-need-hunger-low             = {CAPITALIZE(SUBJECT($entity))} {CONJUGATE-BASIC($entity, "look", "looks")} hungry.
examinable-need-hunger-low-meme        = {CAPITALIZE(SUBJECT($entity))} {CONJUGATE-BASIC($entity, "look", "looks")} hungie.
examinable-need-hunger-critical        = {CAPITALIZE(SUBJECT($entity))} {CONJUGATE-BASIC($entity, "look", "looks")} starved!
examinable-need-hunger-unused          = {CAPITALIZE(SUBJECT($entity))} {CONJUGATE-BASIC($entity, "look", "looks")} like they dont actually get hungry!

examinable-need-hunger-extrasatisfied-self  = You feel nice and full!
examinable-need-hunger-satisfied-self       = You feel well-fed.
examinable-need-hunger-low-self             = You feel a bit hungry.
examinable-need-hunger-low-self-meme        = You feel a bit hungie.
examinable-need-hunger-critical-self        = You feel absolutely starving!
examinable-need-hunger-numberized           = You'd rate {CAPITALIZE(SUBJECT($entity))}'s satiation as {$current}/{$max}.
examinable-need-hunger-numberized-self      = You'd rate your satiation as {$current}/{$max}.

examinable-need-hunger-timeleft-extrasatisfied-self   = You feel like you will remain stuffed for...
examinable-need-hunger-timeleft-satisfied-self        = You feel like you will remain well-fed for...
examinable-need-hunger-timeleft-low-self              = You feel like you could put up with your hunger for....
examinable-need-hunger-timeleft-low-meme-self         = You feel like you could put up with your hungieness for....
examinable-need-hunger-timeleft-critical-self         = {CAPITALIZE($creature)} needs food badly!

examinable-need-hunger-timeleft-extrasatisfied = You feel like {CAPITALIZE(SUBJECT($entity))} will remain stuffed for...
examinable-need-hunger-timeleft-satisfied      = You feel like {CAPITALIZE(SUBJECT($entity))} will remain well-fed for....
examinable-need-hunger-timeleft-low            = You feel like {CAPITALIZE(SUBJECT($entity))} could put up with their hunger for...
examinable-need-hunger-timeleft-low-meme       = You feel like {CAPITALIZE(SUBJECT($entity))} could put up with their hungieness for...
examinable-need-hunger-timeleft-critical       = {CAPITALIZE($creature)} needs food badly!

# Thirst
examinable-need-thirst-extrasatisfied  = {CAPITALIZE(SUBJECT($entity))} {CONJUGATE-BASIC($entity, "look", "looks")} well hydrated!
examinable-need-thirst-satisfied       = {CAPITALIZE(SUBJECT($entity))} {CONJUGATE-BASIC($entity, "look", "looks")} quenched.
examinable-need-thirst-low             = {CAPITALIZE(SUBJECT($entity))} {CONJUGATE-BASIC($entity, "look", "looks")} thirsty.
examinable-need-thirst-low-meme        = {CAPITALIZE(SUBJECT($entity))} {CONJUGATE-BASIC($entity, "look", "looks")} thurrsty.
examinable-need-thirst-critical        = {CAPITALIZE(SUBJECT($entity))} {CONJUGATE-BASIC($entity, "look", "looks")} dehydrated!
examinable-need-thirst-unused          = {CAPITALIZE(SUBJECT($entity))} {CONJUGATE-BASIC($entity, "look", "looks")} like they dont actually get thirsty!

examinable-need-thirst-extrasatisfied-self  = You feel nice and hydrated!
examinable-need-thirst-satisfied-self       = You feel quenched.
examinable-need-thirst-low-self             = You feel a bit thirsty.
examinable-need-thirst-low-self-meme        = You feel a bit thurrsty.
examinable-need-thirst-critical-self        = You feel absolutely parched!
examinable-need-thirst-numberized           = You'd rate {CAPITALIZE(SUBJECT($entity))}'s hydration as {$current}/{$max}.
examinable-need-thirst-numberized-self      = You'd rate your hydration as {$current}/{$max}.

examinable-need-thirst-timeleft-extrasatisfied-self  = You feel like {CAPITALIZE(SUBJECT($entity))} will remain hydrated for...
examinable-need-thirst-timeleft-satisfied-self       = You feel like {CAPITALIZE(SUBJECT($entity))} will remain quenched for...
examinable-need-thirst-timeleft-low-self             = You feel like {CAPITALIZE(SUBJECT($entity))} could put up with their thirst for...
examinable-need-thirst-timeleft-low-meme-self        = You feel like {CAPITALIZE(SUBJECT($entity))} could put up with their thurrstyness for...
examinable-need-thirst-timeleft-critical-self        = {CAPITALIZE($creature)} need

examinable-need-thirst-timeleft-extrasatisfied  = You feel like you will remain hydrated for...\n.
examinable-need-thirst-timeleft-satisfied       = You feel like you will remain quenched for...\n.
examinable-need-thirst-timeleft-low             = You feel like you could put up with your thirst for...\n.
examinable-need-thirst-timeleft-low-meme        = You feel like you could put up with your thurrstyness for...\n.
examinable-need-thirst-timeleft-critical        = {CAPITALIZE($creature)} needs water badly!

# Buffs and Debuffs
examinable-need-effect-header = [bold][underline]Effects:[/underline][/bold]
examinable-need-effect-buff = [color=green]{kind}[/color] {text} [color=green]({amount})[/color]
examinable-need-effect-debuff = [color=red]{kind}[/color] {text} [color=red]({amount})[/color]
examinable-need-effect-buff-custom = [color=green]{kind}[/color] {text}
examinable-need-effect-debuff-custom = [color=red]{kind}[/color] {text}
