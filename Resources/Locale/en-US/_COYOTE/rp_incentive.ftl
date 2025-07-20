#coyote-rp-incentive-payward-message =
#    Incentive payward deposited:
#    { ($hasModifier) ->
#    *[false] ${ $amount }
#    [true] {( $hasMultiplier ) ->
#        *[false] [bold]{ $basePay } x { $multiplier } = [bold]{ $amount }[/bold]
#        [true] {( $hasAddedPay ) ->
#            *[false] [bold]{ $basePay } + { $addedPay } x { $multiplier } = [bold]{ $amount }[/bold]
#            [true] [bold]{ $basePay } + { $addedPay } x { $multiplier } = [bold]{ $amount }[/bold]
#        }
#    }
#
#
## ({ $basePay } + { $addedPay }) x { $multiplier } = [bold]{ $amount }[/bold]

coyote-rp-incentive-payward-message = Incentive payward deposited: ${ $amount }
coyote-rp-incentive-payward-message-multiplier = Incentive payward deposited: { $basePay } x { $multiplier } = [bold]{ $amount }[/bold]
coyote-rp-incentive-payward-message-additive = Incentive payward deposited: { $basePay } + { $addedPay } = [bold]{ $amount }[/bold]
coyote-rp-incentive-payward-message-multiplier-and-additive = Incentive payward deposited: ({ $basePay } + { $addedPay }) x { $multiplier } = [bold]{ $amount }[/bold]
