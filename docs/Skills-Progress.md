# Progress of implemented Skills

I want to give a overview about which skills are already implemented and which
ones need some rework.

Some general notes:

* Elemental effects (poison, ice, etc.) are not yet implemented at all

* Area Damage Skills with explicit damage packets work, but are vulnerable to cheaters

* Area Damage Skills with implicit damage work, but directions are not
  considered yet - they hit targets in all directions.

* All kind of skills which are somehow special are not working

## Normal Skills

| Number | Name                        | State                                                            |
|--------|-----------------------------|------------------------------------------------------------------|
| 1      | Poison                      | Damage works, poisoning not yet                                  |
| 2      | Meteorite                   | works                                                            |
| 3      | Lightning                   | works                                                            |
| 4      | Fire Ball                   | works                                                            |
| 5      | Flame                       | works                                                            |
| 6      | Teleport                    | works                                                            |
| 7      | Ice                         | Damage works, icing not yet                                      |
| 8      | Twister                     | works                                                            |
| 9      | Evil Spirit                 | works                                                            |
| 10     | Hellfire                    | works                                                            |
| 11     | Power Wave                  | works                                                            |
| 12     | Aqua Beam                   | works                                                            |
| 13     | Cometfall                   | works                                                            |
| 14     | Inferno                     | works                                                            |
| 15     | Teleport Ally               | not implemented yet                                              |
| 16     | Soul Barrier                | works                                                            |
| 17     | Energy Ball                 | works                                                            |
| 18     | Defense                     | works, untested                                                  |
| 19     | Falling Slash               | works                                                            |
| 20     | Lunge                       | works                                                            |
| 21     | Uppercut                    | works                                                            |
| 22     | Cyclone                     | works                                                            |
| 23     | Slash                       | works                                                            |
| 24     | Triple Shot                 | works,                                                           |
| 26     | Heal                        | works                                                            |
| 27     | Greater Defense             | works                                                            |
| 28     | Greater Damage              | works                                                            |
| 30     | Summon Goblin               | not implemented yet                                              |
| 31     | Summon Stone Golem          |                                                                  |
| 32     | Summon Assassin             |                                                                  |
| 33     | Summon Elite Yeti           |                                                                  |
| 34     | Summon Dark Knight          |                                                                  |
| 35     | Summon Bali                 |                                                                  |
| 36     | Summon Soldier              |                                                                  |
| 38     | Decay                       | works, poison isn't applied yet                                  |
| 39     | Ice Storm                   | works, icing isn't applied yet                                   |
| 40     | Nova                        | not implemented yet                                              |
| 41     | Twisting Slash              | works                                                            |
| 42     | Rageful Blow                | works                                                            |
| 43     | Death Stab                  | works                                                            |
| 44     | Crescent Moon Slash         | works, untested                                                  |
| 45     | Lance                       | ?                                                                |
| 46     | Starfall                    | ?                                                                |
| 47     | Impale                      | works                                                            |
| 48     | Swell Life                  | works                                                            |
| 49     | Fire Breath                 | works                                                            |
| 50     | Flame of Evil (Monster)     |                                                                  |
| 51     | Ice Arrow                   | works, icing isn't applied yet                                   |
| 52     | Penetration                 | works, untested                                                  |
| 55     | Fire Slash                  | ?                                                                |
| 56     | Power Slash                 | probably not working right; needs to take direction into account |
| 57     | Spiral Slash                | ?                                                                |
| 60     | Force                       | works                                                            |
| 61     | Fire Burst                  | works                                                            |
| 62     | Earthshake                  | not implemented                                                  |
| 63     | Summon                      | not implemented                                                  |
| 64     | Increase Critical Damage    | not implemented                                                  |
| 65     | Electric Spike              | ?                                                                |
| 66     | Force Wave                  | works                                                            |
| 67     | Stun                        | not implemented                                                  |
| 68     | Cancel Stun                 | not implemented                                                  |
| 69     | Swell Mana                  | not implemented                                                  |
| 70     | Invisibility                | not implemented                                                  |
| 71     | Cancel Invisibility         | not implemented                                                  |
| 72     | Abolish Magic               | not implemented                                                  |
| 73     | Mana Rays                   | ?                                                                |
| 74     | Fire Blast                  | ?                                                                |
| 76     | Plasma Storm                | ?                                                                |
| 77     | Infinity Arrow              | not implemented                                                  |
| 78     | Fire Scream                 | ?                                                                |
| 79     | Explosion                   | ?                                                                |
| 200    | Summon Monster              | ?                                                                |
| 201    | Magic Attack Immunity       | not implemented                                                  |
| 202    | Physical Attack Immunity    | not implemented                                                  |
| 203    | Potion of Bless             | not implemented                                                  |
| 204    | Potion of Soul              | not implemented                                                  |
| 210    | Spell of Protection         | not implemented                                                  |
| 211    | Spell of Restriction        | not implemented                                                  |
| 212    | Spell of Pursuit            | not implemented                                                  |
| 213    | Shield-Burn                 | not implemented                                                  |
| 214    | Drain Life                  | not implemented                                                  |
| 215    | Chain Lightning             | ?                                                                |
| 217    | Damage Reflection           | not implemented                                                  |
| 218    | Berserker                   | ?                                                                |
| 219    | Sleep                       | not implemented                                                  |
| 221    | Weakness                    | not implemented                                                  |
| 222    | Innovation                  | not implemented                                                  |
| 223    | Explosion                   | ?                                                                |
| 224    | Requiem                     | ?                                                                |
| 225    | Pollution                   | ?                                                                |
| 230    | Lightning Shock             | ?                                                                |
| 232    | Strike of Destruction       | works, untested                                                  |
| 233    | Expansion of Wizardry       | not implemented                                                  |
| 234    | Recovery                    | works                                                            |
| 235    | Multi-Shot                  | works, untested                                                  |
| 236    | Flame Strike                | ?                                                                |
| 237    | Gigantic Storm              | works, untested                                                  |
| 238    | Chaotic Diseier             | works, but needs to take direction into account                  |
| 239    | Doppelganger Self Explosion | ?                                                                |
| 260    | Killing Blow                | ?                                                                |
| 261    | Beast Uppercut              | ?                                                                |
| 262    | Chain Drive                 | ?                                                                |
| 263    | Dark Side                   | ?                                                                |
| 264    | Dragon Roar                 | ?                                                                |
| 265    | Dragon Slasher              | ?                                                                |
| 266    | Ignore Defense              | not implemented                                                  |
| 267    | Increase Health             | not implemented                                                  |
| 268    | Increase Block              | not implemented                                                  |
| 269    | Charge                      | ?                                                                |
| 270    | Phoenix Shot                | ?                                                                |

## Master Skills

Note: Some skills have the state *unused*. They're not used in the game at
season 6, but probably in later seasons. Therefore, they are also *not implemented*.

| Number | Name                          | State           |
|--------|-------------------------------|-----------------|
| 300    | Durability Reduction (1)      | not implemented |
| 301    | PvP Defence Rate Inc          | works           |
| 302    | Maximum SD increase           | works           |
| 303    | Automatic Mana Rec Inc        | works           |
| 304    | Poison Resistance Inc         | not implemented |
| 305    | Durability Reduction (2)      | not implemented |
| 306    | SD Recovery Speed Inc         | not implemented |
| 307    | Automatic HP Rec Inc          | works           |
| 308    | Lightning Resistance Inc      | not implemented |
| 309    | Defense Increase              | works           |
| 310    | Automatic AG Rec Inc          | works           |
| 311    | Ice Resistance Increase       | not implemented |
| 312    | Durability Reduction (3)      | not implemented |
| 313    | Defense Success Rate Inc      | works           |
| 314    | Cast Invincibility            | unused          |
| 315    | Armor Set Bonus Inc           | unused          |
| 316    | Vengeance                     | unused          |
| 317    | Energy Increase               | unused          |
| 318    | Stamina Increase              | unused          |
| 319    | Agility Increase              | unused          |
| 320    | Strength Increase             | unused          |
| 321    | Wing of Storm Abs PowUp       | unused          |
| 322    | Wing of Storm Def PowUp       | unused          |
| 323    | Iron Defense                  | unused          |
| 324    | Wing of Storm Att PowUp       | unused          |
| 325    | Attack Succ Rate Inc          | works           |
| 326    | Cyclone Strengthener          | works, untested |
| 327    | Slash Strengthener            | works, untested |
| 328    | Falling Slash Streng          | works, untested |
| 329    | Lunge Strengthener            | works, untested |
| 330    | Twisting Slash Streng         | works, untested |
| 331    | Rageful Blow Streng           | works, untested |
| 332    | Twisting Slash Mastery        | works, untested |
| 333    | Rageful Blow Mastery          | works, untested |
| 334    | Maximum Life Increase         | works           |
| 335    | Weapon Mastery                |                 |
| 336    | Death Stab Strengthener       | works, untested |
| 337    | Strike of Destr Str           | works, untested |
| 338    | Maximum Mana Increase         | works           |
| 339    | Death Stab Proficiency        | unused          |
| 340    | Strike of Destr Prof          | unused          |
| 341    | Maximum AG Increase           | unused          |
| 342    | Death Stab Mastery            | unused          |
| 343    | Strike of Destr Mast          | unused          |
| 344    | Blood Storm                   | unused          |
| 345    | Combo Strengthener            | unused          |
| 346    | Blood Storm Strengthener      | unused          |
| 347    | PvP Attack Rate               | works           |
| 348    | Two-handed Sword Stren        | not implemented |
| 349    | One-handed Sword Stren        | not implemented |
| 350    | Mace Strengthener             | not implemented |
| 351    | Spear Strengthener            | not implemented |
| 352    | Two-handed Sword Mast         | works           |
| 353    | One-handed Sword Mast         | not implemented |
| 354    | Mace Mastery                  | not implemented |
| 355    | Spear Mastery                 | not implemented |
| 356    | Swell Life Strengt            | not implemented |
| 357    | Mana Reduction                | not implemented |
| 358    | Monster Attack SD Inc         | works           |
| 359    | Monster Attack Life Inc       | works           |
| 360    | Swell Life Proficiency        | not implemented |
| 361    | Minimum Attack Power Inc      | works           |
| 362    | Monster Attack Mana Inc       | works           |
| 363    | Swell Life Mastery            | unused          |
| 364    | Maximum Attack Power Inc      | unused          |
| 366    | Inc crit damage rate          | unused          |
| 367    | Restores all Mana             | unused          |
| 368    | Restores all HP               | unused          |
| 369    | Inc exc damage rate           | unused          |
| 370    | Inc double damage rate        | unused          |
| 371    | Inc chance of ignore Def      | unused          |
| 372    | Restores all SD               | unused          |
| 373    | Inc triple damage rate        | unused          |
| 374    | Eternal Wings Abs PowUp       | unused          |
| 375    | Eternal Wings Def PowUp       | unused          |
| 377    | Eternal Wings Att PowUp       | unused          |
| 378    | Flame Strengthener            | not implemented |
| 379    | Lightning Strengthener        | not implemented |
| 380    | Expansion of Wiz Streng       | not implemented |
| 381    | Inferno Strengthener          | not implemented |
| 382    | Blast Strengthener            | not implemented |
| 383    | Expansion of Wiz Mas          | not implemented |
| 384    | Poison Strengthener           | not implemented |
| 385    | Evil Spirit Streng            | not implemented |
| 386    | Magic Mastery                 | not implemented |
| 387    | Decay Strengthener            | not implemented |
| 388    | Hellfire Strengthener         | not implemented |
| 389    | Ice Strengthener              | not implemented |
| 390    | Meteor Strengthener           | unused          |
| 391    | Ice Storm Strengthener        | unused          |
| 392    | Nova Strengthener             | unused          |
| 393    | Ice Storm Mastery             | unused          |
| 394    | Meteor Mastery                | unused          |
| 395    | Nova Cast Strengthener        | unused          |
| 397    | One-handed Staff Stren        | not implemented |
| 398    | Two-handed Staff Stren        | not implemented |
| 399    | Shield Strengthener           | not implemented |
| 400    | One-handed Staff Mast         | works           |
| 401    | Two-handed Staff Mast         | not implemented |
| 402    | Shield Mastery                | not implemented |
| 403    | Soul Barrier Strength         | not implemented |
| 404    | Soul Barrier Proficie         | not implemented |
| 405    | Minimum Wizardry Inc          | not implemented |
| 406    | Soul Barrier Mastery          | unused          |
| 407    | Maximum Wizardry Inc          | unused          |
| 409    | Illusion Wings Abs PowUp      | unused          |
| 410    | Illusion Wings Def PowUp      | unused          |
| 411    | Multi-Shot Streng             | unused          |
| 412    | Illusion Wings Att PowUp      | unused          |
| 413    | Heal Strengthener             | not implemented |
| 414    | Triple Shot Strengthener      | not implemented |
| 415    | Summoned Monster Str (1)      | not implemented |
| 416    | Penetration Strengthener      | not implemented |
| 417    | Defense Increase Str          | not implemented |
| 418    | Triple Shot Mastery           | not implemented |
| 419    | Summoned Monster Str (2)      | not implemented |
| 420    | Attack Increase Str           | not implemented |
| 421    | Weapon Mastery                | not implemented |
| 422    | Attack Increase Mastery       | not implemented |
| 423    | Defense Increase Mastery      | not implemented |
| 424    | Ice Arrow Strengthener        | not implemented |
| 425    | Cure                          | unused          |
| 426    | Party Healing                 | unused          |
| 427    | Poison Arrow                  | unused          |
| 428    | Summoned Monster Str (3)      | unused          |
| 429    | Party Healing Str             | unused          |
| 430    | Bless                         | unused          |
| 431    | Multi-Shot Mastery            | unused          |
| 432    | Summon Satyros                | unused          |
| 433    | Bless Strengthener            | unused          |
| 434    | Poison Arrow Str              | unused          |
| 435    | Bow Strengthener              | not implemented |
| 436    | Crossbow Strengthener         | not implemented |
| 437    | Shield Strengthener           | not implemented |
| 438    | Bow Mastery                   | works           |
| 439    | Crossbow Mastery              | not implemented |
| 440    | Shield Mastery                | not implemented |
| 441    | Infinity Arrow Str            | not implemented |
| 442    | Minimum Att Power Inc         | works           |
| 443    | Maximum Att Power Inc         | unused          |
| 445    | DimensionWings Abs PowUp      | unused          |
| 446    | DimensionWings Def PowUp      | unused          |
| 447    | DimensionWings Att PowUp      | unused          |
| 448    | Fire Tome Strengthener        | not implemented |
| 449    | Wind Tome Strengthener        | not implemented |
| 450    | Lightning Tome Stren          | not implemented |
| 451    | Fire Tome Mastery             | not implemented |
| 452    | Wind Tome Mastery             | not implemented |
| 453    | Lightning Tome Mastery        | not implemented |
| 454    | Sleep Strengthener            | not implemented |
| 455    | Chain Lightning Str           | not implemented |
| 456    | Lightning Shock Str           | not implemented |
| 457    | Magic Mastery                 | not implemented |
| 458    | Drain Life Strengthener       | not implemented |
| 459    | Weakness Strengthener         | unused          |
| 460    | Innovation Strengthener       | unused          |
| 461    | Blind                         | unused          |
| 462    | Drain Life Mastery            | unused          |
| 463    | Blind Strengthener            | unused          |
| 465    | Stick Strengthener            | not implemented |
| 466    | Other World Tome Streng       | not implemented |
| 467    | Stick Mastery                 | not implemented |
| 468    | Other World Tome Mastery      | works           |
| 469    | Berserker Strengthener        | not implemented |
| 470    | Berserker Proficiency         | not implemented |
| 471    | Minimum Wiz/Curse Inc         | works           |
| 472    | Berserker Mastery             | unused          |
| 473    | Maximum Wiz/Curse Inc         | unused          |
| 475    | Wing of Ruin Abs PowUp        | unused          |
| 476    | Wing of Ruin Def PowUp        | unused          |
| 478    | Wing of Ruin Att PowUp        | unused          |
| 479    | Cyclone Strengthener          | not implemented |
| 480    | Lightning Strengthener        | not implemented |
| 481    | Twisting Slash Stren          | not implemented |
| 482    | Power Slash Streng            | not implemented |
| 483    | Flame Strengthener            | not implemented |
| 484    | Blast Strengthener            | not implemented |
| 485    | Weapon Mastery                | not implemented |
| 486    | Inferno Strengthener          | not implemented |
| 487    | Evil Spirit Strengthen        | not implemented |
| 488    | Magic Mastery                 | not implemented |
| 489    | Ice Strengthener              | not implemented |
| 490    | Blood Attack Strengthen       | not implemented |
| 491    | Ice Mastery                   | unused          |
| 492    | Flame Strike Strengthen       | unused          |
| 493    | Fire Slash Mastery            | unused          |
| 494    | Flame Strike Mastery          | unused          |
| 495    | Earth Prison                  | unused          |
| 496    | Gigantic Storm Str            | unused          |
| 497    | Earth Prison Str              | unused          |
| 504    | Emperor Cape Abs PowUp        | unused          |
| 505    | Emperor Cape Def PowUp        | unused          |
| 506    | Adds Command Stat             | unused          |
| 507    | Emperor Cape Att PowUp        | unused          |
| 508    | Fire Burst Streng             | not implemented |
| 509    | Force Wave Streng             | not implemented |
| 510    | Dark Horse Streng (1)         | not implemented |
| 511    | Critical DMG Inc PowUp        | not implemented |
| 512    | Earthshake Streng             | not implemented |
| 513    | Weapon Mastery                | not implemented |
| 514    | Fire Burst Mastery            | not implemented |
| 515    | Crit DMG Inc PowUp (2)        | not implemented |
| 516    | Earthshake Mastery            | not implemented |
| 517    | Crit DMG Inc PowUp (3)        | not implemented |
| 518    | Fire Scream Stren             | not implemented |
| 519    | Electric Spark Streng         | unused          |
| 520    | Fire Scream Mastery           | unused          |
| 521    | Iron Defense                  | unused          |
| 522    | Critical Damage Inc M         | unused          |
| 523    | Chaotic Diseier Str           | unused          |
| 524    | Iron Defense Str              | unused          |
| 526    | Dark Spirit Str               | not implemented |
| 527    | Scepter Strengthener          | not implemented |
| 528    | Shield Strengthener           | not implemented |
| 529    | Use Scepter : Pet Str         | not implemented |
| 530    | Dark Spirit Str (2)           | not implemented |
| 531    | Scepter Mastery               | not implemented |
| 532    | Shield Mastery                | not implemented |
| 533    | Command Attack Inc            | not implemented |
| 534    | Dark Spirit Str (3)           | not implemented |
| 535    | Pet Durability Str            | not implemented |
| 536    | Dark Spirit Str (4)           | unused          |
| 538    | Dark Spirit Str (5)           | unused          |
| 539    | Spirit Lord                   | unused          |
| 551    | Killing Blow Strengthener     | not implemented |
| 552    | Beast Uppercut Strengthener   | not implemented |
| 554    | Killing Blow Mastery          | not implemented |
| 555    | Beast Uppercut Mastery        | not implemented |
| 557    | Weapon Mastery                | not implemented |
| 558    | Chain Drive Strengthener      | not implemented |
| 559    | Dark Side Strengthener        | not implemented |
| 560    | Dragon Roar Strengthener      | not implemented |
| 568    | Equipped Weapon Strengthener  | not implemented |
| 569    | Def SuccessRate IncPowUp      | not implemented |
| 571    | Equipped Weapon Mastery       | not implemented |
| 572    | DefSuccessRate IncMastery     | not implemented |
| 573    | Stamina Increase Strengthener | not implemented |
| 578    | Durability Reduction (1)      | not implemented |
| 579    | Increase PvP Defense Rate     | works           |
| 580    | Increase Maximum SD           | works           |
| 581    | Increase Mana Recovery Rate   | works           |
| 582    | Increase Poison Resistance    | not implemented |
| 583    | Durability Reduction (2)      | not implemented |
| 584    | Increase SD Recovery Rate     | works           |
| 585    | Increase HP Recovery Rate     | works           |
| 586    | Increase Lightning Resistance | not implemented |
| 587    | Increases Defense             | works           |
| 588    | Increases AG Recovery Rate    | not implemented |
| 589    | Increase Ice Resistance       | not implemented |
| 590    | Durability Reduction(3)       | not implemented |
| 591    | Increase Defense Success Rate | works           |
| 592    | Cast Invincibility            | unused          |
| 599    | Increase Attack Success Rate  | works           |
| 600    | Increase Maximum HP           | works           |
| 601    | Increase Maximum Mana         | works           |
| 602    | Increase Maximum AG           | unused          |
| 603    | Increase PvP Attack Rate      | works           |
| 604    | Decrease Mana                 | not implemented |
| 605    | Recover SD from Monster Kills | works           |
| 606    | Recover HP from Monster Kills | works           |
| 607    | Increase Minimum Attack Power | works           |
| 608    | Recover Mana Monster Kills    | works           |
| 609    | Increase Maximum Attack Power | unused          |
| 610    | Increases Crit Damage Chance  | unused          |
| 611    | Recover Mana Fully            | unused          |
| 612    | Recovers HP Fully             | unused          |
| 613    | Increase Exc Damage Chance    | unused          |
| 614    | Increase Double Damage Chance | unused          |
| 615    | Increase Ignore Def Chance    | unused          |
| 616    | Recovers SD Fully             | unused          |
| 617    | Increase Triple Damage Chance | unused          |
