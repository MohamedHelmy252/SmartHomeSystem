using Application.DTOs.FaceRecognition;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "HomeOwner,Admin")]
    public class FaceMembersController : ControllerBase
    {
        private readonly IFaceMemberService _faceMemberService;

        public FaceMembersController(IFaceMemberService faceMemberService)
        {
            _faceMemberService = faceMemberService;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateHomeMemberFaceDTO dto)
        {
            var result = await _faceMemberService.CreateAsync(dto);

            return Ok(new
            {
                success = true,
                message = "Face member added successfully",
                data = result
            });
        }

        [HttpGet("home/{homeId}")]
        public async Task<IActionResult> GetByHome(int homeId)
        {
            var faces = await _faceMemberService.GetByHomeAsync(homeId);

            return Ok(new
            {
                success = true,
                count = faces.Count,
                data = faces
            });
        }

        [HttpDelete("{faceId}")]
        public async Task<IActionResult> Delete(int faceId)
        {
            var deleted = await _faceMemberService.DeleteAsync(faceId);

            if (!deleted)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Face member not found"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Face member deleted successfully"
            });
        }
    }
}