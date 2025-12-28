using System;

namespace Magicka
{
	// Token: 0x02000264 RID: 612
	public enum Animations : ushort
	{
		// Token: 0x040011B1 RID: 4529
		None,
		// Token: 0x040011B2 RID: 4530
		idle,
		// Token: 0x040011B3 RID: 4531
		idlelong,
		// Token: 0x040011B4 RID: 4532
		idlelong0 = 2,
		// Token: 0x040011B5 RID: 4533
		idlelong1,
		// Token: 0x040011B6 RID: 4534
		idlelong2,
		// Token: 0x040011B7 RID: 4535
		idle_agg,
		// Token: 0x040011B8 RID: 4536
		idlelong_agg,
		// Token: 0x040011B9 RID: 4537
		idlelong_agg0 = 6,
		// Token: 0x040011BA RID: 4538
		idlelong_agg1,
		// Token: 0x040011BB RID: 4539
		idlelong_agg2,
		// Token: 0x040011BC RID: 4540
		idle_wnd,
		// Token: 0x040011BD RID: 4541
		idlelong_wnd,
		// Token: 0x040011BE RID: 4542
		idlelong_wnd0 = 10,
		// Token: 0x040011BF RID: 4543
		idlelong_wnd1,
		// Token: 0x040011C0 RID: 4544
		idlelong_wnd2,
		// Token: 0x040011C1 RID: 4545
		idle_grip,
		// Token: 0x040011C2 RID: 4546
		die0,
		// Token: 0x040011C3 RID: 4547
		die1,
		// Token: 0x040011C4 RID: 4548
		die_drown,
		// Token: 0x040011C5 RID: 4549
		die_drown0 = 16,
		// Token: 0x040011C6 RID: 4550
		die_drown1,
		// Token: 0x040011C7 RID: 4551
		die_drown2,
		// Token: 0x040011C8 RID: 4552
		hit,
		// Token: 0x040011C9 RID: 4553
		hit_fly,
		// Token: 0x040011CA RID: 4554
		hit_stun_begin,
		// Token: 0x040011CB RID: 4555
		hit_stun_end,
		// Token: 0x040011CC RID: 4556
		hit_slide,
		// Token: 0x040011CD RID: 4557
		attack_hit,
		// Token: 0x040011CE RID: 4558
		attack_melee0,
		// Token: 0x040011CF RID: 4559
		attack_melee1,
		// Token: 0x040011D0 RID: 4560
		attack_melee2,
		// Token: 0x040011D1 RID: 4561
		attack_melee3,
		// Token: 0x040011D2 RID: 4562
		attack_melee4,
		// Token: 0x040011D3 RID: 4563
		attack_ranged0,
		// Token: 0x040011D4 RID: 4564
		attack_ranged1,
		// Token: 0x040011D5 RID: 4565
		attack_ranged2,
		// Token: 0x040011D6 RID: 4566
		attack_ranged3,
		// Token: 0x040011D7 RID: 4567
		attack_recoil,
		// Token: 0x040011D8 RID: 4568
		block,
		// Token: 0x040011D9 RID: 4569
		boost,
		// Token: 0x040011DA RID: 4570
		charge_area,
		// Token: 0x040011DB RID: 4571
		charge_area_loop,
		// Token: 0x040011DC RID: 4572
		charge_force,
		// Token: 0x040011DD RID: 4573
		charge_force_loop,
		// Token: 0x040011DE RID: 4574
		cast_area_blast,
		// Token: 0x040011DF RID: 4575
		cast_area_fireworks,
		// Token: 0x040011E0 RID: 4576
		cast_area_ground,
		// Token: 0x040011E1 RID: 4577
		cast_area_lightning,
		// Token: 0x040011E2 RID: 4578
		cast_area_push,
		// Token: 0x040011E3 RID: 4579
		cast_charge,
		// Token: 0x040011E4 RID: 4580
		cast_force_lightning,
		// Token: 0x040011E5 RID: 4581
		cast_force_projectile,
		// Token: 0x040011E6 RID: 4582
		cast_force_push,
		// Token: 0x040011E7 RID: 4583
		cast_force_railgun,
		// Token: 0x040011E8 RID: 4584
		cast_force_shield,
		// Token: 0x040011E9 RID: 4585
		cast_force_spray,
		// Token: 0x040011EA RID: 4586
		cast_force_zap,
		// Token: 0x040011EB RID: 4587
		cast_magick_global,
		// Token: 0x040011EC RID: 4588
		cast_magick_direct,
		// Token: 0x040011ED RID: 4589
		cast_magick_self,
		// Token: 0x040011EE RID: 4590
		cast_magick_sweep,
		// Token: 0x040011EF RID: 4591
		cast_self,
		// Token: 0x040011F0 RID: 4592
		cast_sword,
		// Token: 0x040011F1 RID: 4593
		cast_sword_lightning,
		// Token: 0x040011F2 RID: 4594
		cast_sword_projectile,
		// Token: 0x040011F3 RID: 4595
		cast_sword_railgun,
		// Token: 0x040011F4 RID: 4596
		cast_sword_shield,
		// Token: 0x040011F5 RID: 4597
		cast_sword_spray,
		// Token: 0x040011F6 RID: 4598
		cast_teleport,
		// Token: 0x040011F7 RID: 4599
		cast_spell0,
		// Token: 0x040011F8 RID: 4600
		cast_spell1,
		// Token: 0x040011F9 RID: 4601
		cast_spell2,
		// Token: 0x040011FA RID: 4602
		cast_spell3,
		// Token: 0x040011FB RID: 4603
		cast_spell4,
		// Token: 0x040011FC RID: 4604
		cast_spell5,
		// Token: 0x040011FD RID: 4605
		cast_spell6,
		// Token: 0x040011FE RID: 4606
		cast_spell7,
		// Token: 0x040011FF RID: 4607
		cast_spell8,
		// Token: 0x04001200 RID: 4608
		cast_spell9,
		// Token: 0x04001201 RID: 4609
		chant,
		// Token: 0x04001202 RID: 4610
		cutscene_fall,
		// Token: 0x04001203 RID: 4611
		cutscene_eat,
		// Token: 0x04001204 RID: 4612
		cutscene_eaten,
		// Token: 0x04001205 RID: 4613
		cutscene_sit,
		// Token: 0x04001206 RID: 4614
		cutscene_sit_talk0,
		// Token: 0x04001207 RID: 4615
		cutscene_sit_talk1,
		// Token: 0x04001208 RID: 4616
		cutscene_rise,
		// Token: 0x04001209 RID: 4617
		cutscene_hide,
		// Token: 0x0400120A RID: 4618
		cutscene_unhide,
		// Token: 0x0400120B RID: 4619
		cutscene_cheer,
		// Token: 0x0400120C RID: 4620
		cutscene_complete0,
		// Token: 0x0400120D RID: 4621
		cutscene_complete1,
		// Token: 0x0400120E RID: 4622
		cutscene_complete2,
		// Token: 0x0400120F RID: 4623
		cutscene_complete3,
		// Token: 0x04001210 RID: 4624
		cutscene_fail,
		// Token: 0x04001211 RID: 4625
		cutscene_teleport_in,
		// Token: 0x04001212 RID: 4626
		cutscene_teleport_out,
		// Token: 0x04001213 RID: 4627
		cutscene_introduction,
		// Token: 0x04001214 RID: 4628
		cutscene_idle,
		// Token: 0x04001215 RID: 4629
		cutscene_talkfriendly0,
		// Token: 0x04001216 RID: 4630
		cutscene_talkfriendly1,
		// Token: 0x04001217 RID: 4631
		cutscene_talkidle,
		// Token: 0x04001218 RID: 4632
		cutscene_talkagg0,
		// Token: 0x04001219 RID: 4633
		cutscene_talkagg1,
		// Token: 0x0400121A RID: 4634
		cutscene_talkagg2,
		// Token: 0x0400121B RID: 4635
		cutscene_walklaugh,
		// Token: 0x0400121C RID: 4636
		cutscene_slashvlad,
		// Token: 0x0400121D RID: 4637
		cutscene_slashvladidle,
		// Token: 0x0400121E RID: 4638
		cutscene_lookingforvlad,
		// Token: 0x0400121F RID: 4639
		cutscene_shame,
		// Token: 0x04001220 RID: 4640
		cutscene_woundedkneeling,
		// Token: 0x04001221 RID: 4641
		cutscene_lastbreath,
		// Token: 0x04001222 RID: 4642
		cutscene_stabbed,
		// Token: 0x04001223 RID: 4643
		cutscene_stabbedidle,
		// Token: 0x04001224 RID: 4644
		cutscene_woundedstanding,
		// Token: 0x04001225 RID: 4645
		cutscene_woundedtalk,
		// Token: 0x04001226 RID: 4646
		cutscene_losinghandbegin,
		// Token: 0x04001227 RID: 4647
		cutscene_losinghandidle,
		// Token: 0x04001228 RID: 4648
		cutscene_losinghandend,
		// Token: 0x04001229 RID: 4649
		cutscene_diepose0,
		// Token: 0x0400122A RID: 4650
		cutscene_diepose1,
		// Token: 0x0400122B RID: 4651
		cutscene_flipcoin_begin,
		// Token: 0x0400122C RID: 4652
		cutscene_flipcoin_idle,
		// Token: 0x0400122D RID: 4653
		cutscene_flipcoin_end,
		// Token: 0x0400122E RID: 4654
		cutscene_talkshame_begin,
		// Token: 0x0400122F RID: 4655
		cutscene_talkshame_idle,
		// Token: 0x04001230 RID: 4656
		cutscene_talkshame_end,
		// Token: 0x04001231 RID: 4657
		emote_talk0,
		// Token: 0x04001232 RID: 4658
		emote_point0,
		// Token: 0x04001233 RID: 4659
		emote_terrified0,
		// Token: 0x04001234 RID: 4660
		emote_horrified0,
		// Token: 0x04001235 RID: 4661
		emote_sigh0,
		// Token: 0x04001236 RID: 4662
		emote_rejoice0,
		// Token: 0x04001237 RID: 4663
		emote_rejoice1,
		// Token: 0x04001238 RID: 4664
		emote_confused0,
		// Token: 0x04001239 RID: 4665
		emote_confused1,
		// Token: 0x0400123A RID: 4666
		emote_confused2,
		// Token: 0x0400123B RID: 4667
		emote_confused3,
		// Token: 0x0400123C RID: 4668
		move_walk,
		// Token: 0x0400123D RID: 4669
		move_wnd,
		// Token: 0x0400123E RID: 4670
		move_run,
		// Token: 0x0400123F RID: 4671
		move_sprint,
		// Token: 0x04001240 RID: 4672
		move_panic,
		// Token: 0x04001241 RID: 4673
		move_stumble,
		// Token: 0x04001242 RID: 4674
		move_fall,
		// Token: 0x04001243 RID: 4675
		move_jump_begin,
		// Token: 0x04001244 RID: 4676
		move_jump_mid,
		// Token: 0x04001245 RID: 4677
		move_jump_up,
		// Token: 0x04001246 RID: 4678
		move_jump_down,
		// Token: 0x04001247 RID: 4679
		move_jump_end,
		// Token: 0x04001248 RID: 4680
		pickup_weapon,
		// Token: 0x04001249 RID: 4681
		pickup_staff,
		// Token: 0x0400124A RID: 4682
		pickup_magick,
		// Token: 0x0400124B RID: 4683
		revive,
		// Token: 0x0400124C RID: 4684
		talk_casual0,
		// Token: 0x0400124D RID: 4685
		talk_casual1,
		// Token: 0x0400124E RID: 4686
		talk_casual2,
		// Token: 0x0400124F RID: 4687
		talk_casual3,
		// Token: 0x04001250 RID: 4688
		talk_casual4,
		// Token: 0x04001251 RID: 4689
		talk_alarmed0,
		// Token: 0x04001252 RID: 4690
		talk_alarmed1,
		// Token: 0x04001253 RID: 4691
		talk_alarmed2,
		// Token: 0x04001254 RID: 4692
		talk_alarmed3,
		// Token: 0x04001255 RID: 4693
		talk_greeting0,
		// Token: 0x04001256 RID: 4694
		talk_greeting1,
		// Token: 0x04001257 RID: 4695
		talk_terrified0,
		// Token: 0x04001258 RID: 4696
		talk_terrified1,
		// Token: 0x04001259 RID: 4697
		talk_sad0,
		// Token: 0x0400125A RID: 4698
		talk_sad1,
		// Token: 0x0400125B RID: 4699
		talk_intro0,
		// Token: 0x0400125C RID: 4700
		talk_intro1,
		// Token: 0x0400125D RID: 4701
		talk_intro2,
		// Token: 0x0400125E RID: 4702
		use,
		// Token: 0x0400125F RID: 4703
		spawn,
		// Token: 0x04001260 RID: 4704
		spawn_woundedkneeling,
		// Token: 0x04001261 RID: 4705
		despawn,
		// Token: 0x04001262 RID: 4706
		removestatus0,
		// Token: 0x04001263 RID: 4707
		removestatus1,
		// Token: 0x04001264 RID: 4708
		removestatus2,
		// Token: 0x04001265 RID: 4709
		spec_alert0,
		// Token: 0x04001266 RID: 4710
		spec_alert1,
		// Token: 0x04001267 RID: 4711
		spec_alert2,
		// Token: 0x04001268 RID: 4712
		spec_alert3,
		// Token: 0x04001269 RID: 4713
		spec_action0,
		// Token: 0x0400126A RID: 4714
		spec_action1,
		// Token: 0x0400126B RID: 4715
		spec_action2,
		// Token: 0x0400126C RID: 4716
		spec_action3,
		// Token: 0x0400126D RID: 4717
		spec_action4,
		// Token: 0x0400126E RID: 4718
		spec_arm,
		// Token: 0x0400126F RID: 4719
		spec_disarm,
		// Token: 0x04001270 RID: 4720
		spec_materialize,
		// Token: 0x04001271 RID: 4721
		spec_dormant,
		// Token: 0x04001272 RID: 4722
		spec_etherealize,
		// Token: 0x04001273 RID: 4723
		spec_grapple,
		// Token: 0x04001274 RID: 4724
		spec_entangled,
		// Token: 0x04001275 RID: 4725
		spec_entangled_attack,
		// Token: 0x04001276 RID: 4726
		spec_sit_sleep,
		// Token: 0x04001277 RID: 4727
		spec_sit,
		// Token: 0x04001278 RID: 4728
		spec_throwitem,
		// Token: 0x04001279 RID: 4729
		spec_musician_microphone,
		// Token: 0x0400127A RID: 4730
		spec_musician_drum,
		// Token: 0x0400127B RID: 4731
		spec_musician_saxophone,
		// Token: 0x0400127C RID: 4732
		spec_ghost_drink,
		// Token: 0x0400127D RID: 4733
		spec_ghost_poisoned,
		// Token: 0x0400127E RID: 4734
		spec_wounded_idle,
		// Token: 0x0400127F RID: 4735
		spec_wounded_talk0,
		// Token: 0x04001280 RID: 4736
		spec_wounded_talk1,
		// Token: 0x04001281 RID: 4737
		spec_wounded_talk2,
		// Token: 0x04001282 RID: 4738
		spec_cart_front_enter,
		// Token: 0x04001283 RID: 4739
		spec_cart_front_depart,
		// Token: 0x04001284 RID: 4740
		spec_cart_front_arrive,
		// Token: 0x04001285 RID: 4741
		spec_cart_front_exit,
		// Token: 0x04001286 RID: 4742
		spec_cart_back_enter,
		// Token: 0x04001287 RID: 4743
		spec_cart_back_sit,
		// Token: 0x04001288 RID: 4744
		spec_cart_back_exit,
		// Token: 0x04001289 RID: 4745
		spec_spawn_climbup0,
		// Token: 0x0400128A RID: 4746
		spec_spawn_climbup1,
		// Token: 0x0400128B RID: 4747
		special0,
		// Token: 0x0400128C RID: 4748
		special1,
		// Token: 0x0400128D RID: 4749
		special2,
		// Token: 0x0400128E RID: 4750
		special3,
		// Token: 0x0400128F RID: 4751
		special4,
		// Token: 0x04001290 RID: 4752
		special5,
		// Token: 0x04001291 RID: 4753
		special6,
		// Token: 0x04001292 RID: 4754
		special7,
		// Token: 0x04001293 RID: 4755
		special8,
		// Token: 0x04001294 RID: 4756
		special_getup0,
		// Token: 0x04001295 RID: 4757
		special_getup1,
		// Token: 0x04001296 RID: 4758
		special_delorean,
		// Token: 0x04001297 RID: 4759
		special_lookup,
		// Token: 0x04001298 RID: 4760
		stunned,
		// Token: 0x04001299 RID: 4761
		taunt0,
		// Token: 0x0400129A RID: 4762
		taunt1,
		// Token: 0x0400129B RID: 4763
		taunt2,
		// Token: 0x0400129C RID: 4764
		totalanimations
	}
}
