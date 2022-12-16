
using it.Areas.Admin.Models;
using it.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Xml.Linq;

namespace it.Services
{
	public class Workflow
	{
		protected readonly ItContext _context;

		private readonly IConfiguration _configuration;
		public Workflow(IConfiguration configuration, ItContext context)
		{
			_configuration = configuration;
			_context = context;
		}
		public void create_next(ActivityModel activity)
		{
			var execution = _context.ExecutionModel.Where(d => d.id == activity.execution_id).Include(d => d.process_version).FirstOrDefault();
			var transitions = _context.TransitionModel.Where(d => d.execution_id == activity.execution_id).ToList();
			var activites = _context.ActivityModel.Where(d => d.execution_id == activity.execution_id).ToList();
			var process_version = execution.process_version;
			var process = process_version.process;
			var blocks = process.blocks;
			var links = process.links;

			var node = blocks.Where(d => d.id == activity.block_id).FirstOrDefault();
			if (activity.clazz == "inclusiveGateway")
			{
				var ins = getInEdges(process, node);
				var findTransitions = transitions.Where(d => d.to_activity_id == activity.id).ToList();
				if (findTransitions.Count < ins.Count)
				{
					activity.blocking = true;
				}
				else
				{
					activity.blocking = false;
				}
				_context.Update(activity);
				_context.SaveChanges();
			}

			if (activity.blocking == true)
				return;
			var outs = getOutEdges(process, node);
			if (outs.Count > 0)
			{
				foreach (var outEdge in outs)
				{
					var source = getSource(process, outEdge);
					var target = getTarget(process, outEdge);
					var transition = new TransitionModel()
					{
						label = outEdge.label,
						reverse = outEdge.reverse,
						link_id = outEdge.id,
						execution_id = execution.id,
						from_block_id = source.id,
						to_block_id = target.id,
						from_activity_id = activity.id,
						//to_activity_id: activity.id,
						stt = transitions.Count + 1,
						id = Guid.NewGuid().ToString(),
						created_by = "a76834c7-c4b7-48aa-bf95-05dbd33210ff",
						created_at = DateTime.Now
					};
					_context.Add(transition);
					transitions.Add(transition);

					///CREATE TARGET ACTIVITY
					var create_new = true;
					ActivityModel? activity_new = null;
					if (target.clazz == "inclusiveGateway")
					{
						activity_new = activites.Where(d => d.block_id == target.id).FirstOrDefault();
						if (activity_new != null)
						{
							create_new = false;
						}
					}
					if (create_new == true)
					{
						var blocking = false;
						if (target.clazz == "formTask" || target.clazz == "approveTask" || target.clazz == "mailSystem" || target.clazz == "printSystem")
						{
							blocking = true;
						}

						activity_new = new ActivityModel()
						{
							execution_id = execution.id,
							label = target.label,
							block_id = target.id,
							stt = activites.Count + 1,
							clazz = target.clazz,
							executed = !blocking,
							failed = false,
							blocking = blocking,
							id = Guid.NewGuid().ToString(),
							//fields= fields,
							data_setting = target.data_setting,
							//created_by= that.current_user.id,
							started_at = DateTime.Now
						};
						if (blocking == false)
						{
							activity_new.created_by = "a76834c7-c4b7-48aa-bf95-05dbd33210ff";
							activity_new.created_at = DateTime.Now;
						}
						_context.Add(activity_new);
						activites.Add(activity_new);
					}
					////
					if (activity_new != null)
						transition.to_activity_id = activity_new.id;
					////
					if (activity_new.blocking == true)
					{
						var custom_block = _context.CustomBlockModel.Where(d => d.block_id == activity_new.block_id && d.execution_id == activity_new.execution_id).FirstOrDefault();
						if (custom_block == null)
						{
							var data_setting_block = target.data_setting;
							var type_performer = target.type_performer;
							var data_setting = new CustomBlockSettings() { };
							if (type_performer == 1 && data_setting_block.block_id == null)
							{
								data_setting.type_performer = 4;
								//data_setting.listuser = [that.current_user.id];
							}
							else if (type_performer == 1 && data_setting_block.block_id != null)
							{
								data_setting.type_performer = 4;
								var findActivity = activites.Where(d => d.block_id == data_setting_block.block_id).FirstOrDefault();
								if (findActivity != null)
									data_setting.listuser = new List<string>() { findActivity.created_by };
							}
							else if (type_performer == 4)
							{
								data_setting.type_performer = type_performer;
								data_setting.listuser = data_setting_block.listuser;
							}
							else if (type_performer == 3)
							{
								data_setting.type_performer = type_performer;
								data_setting.listdepartment = data_setting_block.listdepartment;
							}
							else if (type_performer == 5)
							{
								data_setting.type_performer = 4;
								data_setting.listuser = new List<string>() { execution.user_id };
							}
							custom_block = new CustomBlockModel()
							{
								data_setting = data_setting,
								block_id = target.id,
								execution_id = execution.id
							};
							_context.Add(custom_block);
						}
					}
					_context.SaveChanges();
					create_next(activity_new);
				}
			}
		}
		public List<ProcessLinkModel>? getInEdges(ProcessModel process, ProcessBlockModel node)
		{
			//var blocks = process.blocks;
			var links = process.links;
			var node_id = node.id;
			var ins = links.Where(d => d.target == node_id && d.reverse == false).ToList();
			return ins;
		}
		public List<ProcessLinkModel>? getOutEdges(ProcessModel process, ProcessBlockModel node)
		{
			//var blocks = process.blocks;
			var links = process.links;
			var node_id = node.id;
			var ins = links.Where(d => d.source == node_id && d.reverse == false).ToList();
			return ins;
		}

		public ProcessBlockModel? getSource(ProcessModel process, ProcessLinkModel Edge)
		{
			//var blocks = process.blocks;
			var nodes = process.blocks;
			var node = nodes.Where(d => d.id == Edge.source).FirstOrDefault();
			return node;
		}
		public ProcessBlockModel? getTarget(ProcessModel process, ProcessLinkModel Edge)
		{
			//var blocks = process.blocks;
			var nodes = process.blocks;
			var node = nodes.Where(d => d.id == Edge.target).FirstOrDefault();
			return node;
		}
	}

}