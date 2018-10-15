using UnityEngine;
using Bolt;

namespace Bolt.Samples.ClickToMove
{
    public class ClickToMoveController : Bolt.EntityEventListener<ITPCstate>
    {
        public LayerMask validLayers = new LayerMask();
        public Vector3 _destinationPosition = Vector3.zero;
        public CharacterController _cc;

        public override void Attached()
        {
            _cc = GetComponent<CharacterController>();
            state.SetTransforms(state.transform, transform);
        }

        public override void SimulateController()
        {
            IclickToMoveCommandInput input = clickToMoveCommand.Create();
            Vector3 position = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(position);
            RaycastHit[] hits = Physics.RaycastAll(ray, 1000, validLayers);

            if (Input.GetMouseButtonDown(0))
            {
                foreach (RaycastHit hit in hits)
                {
                    BoltLog.Info(hit);
                    if (!hit.collider.isTrigger)
                    {
                        _destinationPosition = hit.point;
                        break;
                    }
                }
            }

            input.click = _destinationPosition;
            entity.QueueInput(input);
        }

        public override void ExecuteCommand(Command command, bool resetState)
        {
            clickToMoveCommand cmd = (clickToMoveCommand)command;

            if (resetState)
            {
                //owner has sent a correction to the controller
                transform.position = cmd.Result.position;
                //_cc.Move(cmd.Result.velocity);
            }
            else
            {

                if (cmd.Input.click != Vector3.zero)
                {
                    if (Vector3.Distance(transform.position, cmd.Input.click) > 0.3f)
                    {
                        transform.LookAt(new Vector3(cmd.Input.click.x, transform.position.y, cmd.Input.click.z));
                        _cc.Move(transform.TransformDirection(new Vector3(0, 0, 1) * 0.01f));
                    }
                }

                cmd.Result.position = transform.position;
                cmd.Result.velocity = GetComponent<CharacterController>().velocity;
            }
        }
    }
}